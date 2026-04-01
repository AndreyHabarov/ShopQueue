using ShopQueue.Application.Repositories;
using ShopQueue.Domain.Entities;
using ShopQueue.Infrastructure.Persistence;

namespace ShopQueue.Infrastructure.Repositories;

public class ShopRepository(AppDbContext db) : IShopRepository
{
    public async Task AddAsync(Shop shop, CancellationToken cancellationToken = default)
    {
        await db.Shops.AddAsync(shop, cancellationToken);
    }

    public async Task<Shop?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await db.Shops.FindAsync([id], cancellationToken);
    }
}