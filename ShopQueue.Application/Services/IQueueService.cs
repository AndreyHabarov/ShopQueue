using ShopQueue.Domain.Entities;

namespace ShopQueue.Application.Services;

public interface IQueueService
{
    Task<Queue> CreateAsync(Guid shopId, string name);
    Task<List<QueueEntry>> GetEntriesAsync(Guid queueId);
    Task<QueueEntry> CallNextAsync(Guid queueId);
}