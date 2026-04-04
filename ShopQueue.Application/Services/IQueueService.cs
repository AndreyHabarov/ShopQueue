using ShopQueue.Domain.Entities;

namespace ShopQueue.Application.Services;

public interface IQueueService
{
    Task<Queue> CreateAsync(Guid shopId, string name, CancellationToken cancellationToken = default);
    Task<List<QueueEntry>> GetEntriesAsync(Guid queueId, CancellationToken cancellationToken = default);
    Task<QueueEntry> CallNextAsync(Guid queueId, CancellationToken cancellationToken = default);
    Task<List<Queue>> GetByShopIdAsync(Guid shopId, CancellationToken cancellationToken = default);
    Task<QueueEntry> MarkAsServedAsync(Guid queueId, Guid entryId, CancellationToken cancellationToken = default);
    Task CancelEntryAsync(Guid queueId, Guid entryId, CancellationToken cancellationToken = default);
}