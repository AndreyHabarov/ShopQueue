using ShopQueue.Domain.Entities;

namespace ShopQueue.Application.Repositories;

public interface IQueueEntryRepository
{
    Task<int> CountWaitingAsync(Guid queueId, CancellationToken cancellationToken = default);
    Task<List<QueueEntry>> GetWaitingAsync(Guid queueId, CancellationToken cancellationToken = default);
    Task<QueueEntry?> GetNextWaitingAsync(Guid queueId, CancellationToken cancellationToken = default);
    Task AddAsync(QueueEntry entry, CancellationToken cancellationToken = default);
}
