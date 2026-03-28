using ShopQueue.Domain.Entities;

namespace ShopQueue.Application.Services;

public interface ICustomerService
{
    Task<QueueEntry> JoinQueueAsync(Guid queueId, string customerName, string customerPhone);
}