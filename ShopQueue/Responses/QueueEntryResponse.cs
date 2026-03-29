using ShopQueue.Domain.Entities;

namespace ShopQueue.Responses;

public record QueueEntryResponse(
    Guid Id,
    Guid QueueId,
    Guid CustomerId,
    string CustomerName,
    int Position,
    string Status,
    DateTime JoinedAt,
    DateTime? CalledAt)
{
    public static QueueEntryResponse FromEntity(QueueEntry entry)
    {
        return new QueueEntryResponse(entry.Id, entry.QueueId, entry.CustomerId, entry.Customer?.Name ?? "",
            entry.Position, entry.Status.ToString(), entry.JoinedAt, entry.CalledAt);
    }

    public static QueueEntryResponse FromEntity(QueueEntry entry, string customerName)
    {
        return new QueueEntryResponse(entry.Id, entry.QueueId, entry.CustomerId, customerName,
            entry.Position, entry.Status.ToString(), entry.JoinedAt, entry.CalledAt);
    }
}