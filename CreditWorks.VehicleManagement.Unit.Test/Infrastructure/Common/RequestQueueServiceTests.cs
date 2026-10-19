using CreditWorks.VehicleManagement.Infrastructure.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace CreditWorks.VehicleManagement.Unit.Test.Infrastructure.Common;

public sealed class RequestQueueServiceTests
{
    [Fact]
    public async Task Worker_runs_accepted_item_and_marks_it_succeeded()
    {
        var queue = new RequestQueueService();
        using var provider = new ServiceCollection().BuildServiceProvider();
        using var worker = new RequestWorkerService(
            queue,
            provider.GetRequiredService<IServiceScopeFactory>(),
            NullLogger<RequestWorkerService>.Instance);
        var invoked = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        await worker.StartAsync(CancellationToken.None);
        try
        {
            Assert.True(queue.TryEnqueue(_ =>
            {
                invoked.SetResult();
                return Task.CompletedTask;
            }, out var id));

            await invoked.Task.WaitAsync(TimeSpan.FromSeconds(5));
            await WaitForState(queue, id, "succeeded");
        }
        finally
        {
            await worker.StopAsync(CancellationToken.None);
        }
    }

    [Fact]
    public async Task Worker_marks_failed_item_and_continues_to_next_item()
    {
        var queue = new RequestQueueService();
        using var provider = new ServiceCollection().BuildServiceProvider();
        using var worker = new RequestWorkerService(
            queue,
            provider.GetRequiredService<IServiceScopeFactory>(),
            NullLogger<RequestWorkerService>.Instance);
        var nextInvoked = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        await worker.StartAsync(CancellationToken.None);
        try
        {
            Assert.True(queue.TryEnqueue(
                _ => Task.FromException(new InvalidOperationException("probe")),
                out var failedId));
            Assert.True(queue.TryEnqueue(_ =>
            {
                nextInvoked.SetResult();
                return Task.CompletedTask;
            }, out var succeededId));

            await nextInvoked.Task.WaitAsync(TimeSpan.FromSeconds(5));
            await WaitForState(queue, failedId, "failed");
            await WaitForState(queue, succeededId, "succeeded");
        }
        finally
        {
            await worker.StopAsync(CancellationToken.None);
        }
    }

    [Fact]
    public void Full_queue_rejects_new_item_without_retaining_its_status()
    {
        var queue = new RequestQueueService();
        for (var index = 0; index < 100; index++)
            Assert.True(queue.TryEnqueue(_ => Task.CompletedTask, out _));

        Assert.False(queue.TryEnqueue(_ => Task.CompletedTask, out var rejectedId));
        Assert.Null(queue.Get(rejectedId));
    }

    private static async Task WaitForState(RequestQueueService queue, Guid id, string expected)
    {
        var deadline = DateTime.UtcNow.AddSeconds(5);
        while (DateTime.UtcNow < deadline)
        {
            if (queue.Get(id)?.State == expected)
                return;
            await Task.Delay(10);
        }

        Assert.Equal(expected, queue.Get(id)?.State);
    }
}
