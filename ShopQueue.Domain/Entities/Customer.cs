namespace ShopQueue.Domain.Entities;

public sealed class Customer
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
}