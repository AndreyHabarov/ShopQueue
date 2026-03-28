namespace ShopQueue.Domain.Enums;

public enum QueueEntryStatus
{
    Waiting,
    Called,
    Served,
    Cancelled
}