using ShopQueue.Domain.Enums;

namespace ShopQueue.Domain.Entities;

public sealed class QueueEntry
{
    public Guid Id { get; set; }
    public Queue Queue { get; set; }
    public Guid QueueId { get; set; }
    public Customer Customer { get; set; }
    public Guid CustomerId { get; set; }
    public int Position { get; set; }
    public QueueEntryStatus Status { get; set; }
    public DateTime JoinedAt { get; set; }
    public DateTime? CalledAt { get; set; }
}