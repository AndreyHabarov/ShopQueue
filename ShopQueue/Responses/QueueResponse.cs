using ShopQueue.Domain.Entities;

namespace ShopQueue.Responses;

public record QueueResponse(Guid Id, Guid ShopId, string Name, bool IsActive, DateTime CreatedAt)
{
    public static QueueResponse FromEntity(Queue queue)
    {
        return new QueueResponse(queue.Id, queue.ShopId, queue.Name, queue.IsActive, queue.CreatedAt);
    }
}