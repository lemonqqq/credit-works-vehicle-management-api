using CreditWorks.VehicleManagement.Application.Contracts;
using CreditWorks.VehicleManagement.Domain.Common;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace CreditWorks.VehicleManagement.Infrastructure.Common
{
    /// <summary>
    ///     Holds accepted work and its status for this application process.
    /// </summary>
    public sealed class RequestQueueService : IRequestQueuedService, IRequestQueueWriteService
    {
        private readonly Channel<QueueItem> _items = Channel.CreateBounded<QueueItem>(
                new BoundedChannelOptions(100) { SingleReader = true });

        private readonly ConcurrentDictionary<Guid, QueueStatus> _queueStatus = new();

        public IAsyncEnumerable<QueueItem> ReadAllAsync(CancellationToken cancellationToken)
            => _items.Reader.ReadAllAsync(cancellationToken);

        /// <summary>
        ///     Queues work to run with services from the worker's own scope.
        /// </summary>
        /// <remarks>Queued items and statuses are lost if the process stops.</remarks>
        public bool TryEnqueue(Func<IServiceProvider, Task> func, out Guid queueId)
        {
            queueId = Guid.NewGuid();
            _queueStatus[queueId] = new QueueStatus("queued");

            if (_items.Writer.TryWrite(new QueueItem(queueId, func)))
                return true;
            _queueStatus.TryRemove(queueId, out _);
            return false;
        }

        public QueueStatus? Get(Guid id)
            => _queueStatus.TryGetValue(id, out var queueStatus) ? queueStatus : null;

        public void Set(Guid id, QueueStatus status) => _queueStatus[id] = status;

        public void Remove(Guid id) => _queueStatus.TryRemove(id, out var _);
    }
}
