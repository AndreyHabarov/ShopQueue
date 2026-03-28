namespace ShopQueue.Application.Messages;

public record ClientCalled(
    Guid QueueEntryId,
    Guid QueueId,
    Guid CustomerId,
    int Position,
    DateTime CalledAt
);