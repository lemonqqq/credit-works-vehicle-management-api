using CreditWorks.VehicleManagement.Domain.Common;

namespace CreditWorks.VehicleManagement.Application.Contracts
{
    public interface IRequestQueuedService
    {
        IAsyncEnumerable<QueueItem> ReadAllAsync(CancellationToken cancellationToken);

        void Set(Guid id, QueueStatus status);
    }

    public interface IRequestQueueWriteService
    {
        bool TryEnqueue(Func<IServiceProvider, Task> work, out Guid queueId);

        QueueStatus? Get(Guid id);

        void Remove(Guid id);
    }
}
