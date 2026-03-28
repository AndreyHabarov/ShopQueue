namespace ShopQueue.Domain.Entities;

public sealed class Queue
{
    public Guid Id { get; set; }
    public Shop Shop { get; set; }
    public Guid ShopId { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}