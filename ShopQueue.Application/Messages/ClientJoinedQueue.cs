namespace ShopQueue.Application.Messages;

public record ClientJoinedQueue(
    Guid QueueEntryId,
    Guid QueueId,
    Guid CustomerId,
    int Position,
    DateTime JoinedAt
);