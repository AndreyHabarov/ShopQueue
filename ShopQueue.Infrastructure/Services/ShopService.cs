using ShopQueue.Application.Services;
using ShopQueue.Domain.Entities;
using ShopQueue.Infrastructure.Persistence;

namespace ShopQueue.Infrastructure.Services;

public class ShopService(AppDbContext db) : IShopService
{
    public async Task<Shop> CreateAsync(string name, string address)
    {
        var shop = new Shop
        {
            Id = Guid.NewGuid(),
            Name = name,
            Address = address,
            CreatedAt = DateTime.UtcNow
        };

        db.Shops.Add(shop);
        await db.SaveChangesAsync();
        return shop;
    }
}