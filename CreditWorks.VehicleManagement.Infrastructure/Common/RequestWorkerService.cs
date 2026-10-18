using CreditWorks.VehicleManagement.Domain.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CreditWorks.VehicleManagement.Infrastructure.Common
{
    public sealed class RequestWorkerService(RequestQueueService requestQueueService, 
        IServiceScopeFactory scopes, 
        ILogger<RequestWorkerService> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await foreach(var item in requestQueueService.Reader.ReadAllAsync(cancellationToken))
            {
                requestQueueService.Set(item.Id, new QueueStatus("running"));
                try
                {
                    await using var scope = scopes.CreateAsyncScope();
                    await item.Run(scope.ServiceProvider);
                    requestQueueService.Set(item.Id, new QueueStatus("succeeded"));
                }
                catch(Exception ex)
                {
                    logger.LogError(ex, "Queue {QueueId} failed", item.Id);
                    requestQueueService.Set(item.Id, new QueueStatus("failed"));
                }
            }
        }
    }
}
