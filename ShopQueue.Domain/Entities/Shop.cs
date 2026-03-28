namespace ShopQueue.Domain.Entities;

public sealed class Shop
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public DateTime CreatedAt { get; set; }
}