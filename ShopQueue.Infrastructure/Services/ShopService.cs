using ShopQueue.Application.Repositories;
using ShopQueue.Application.Services;
using ShopQueue.Domain.Entities;

namespace ShopQueue.Infrastructure.Services;

public class ShopService(IShopRepository shopRepository, IUnitOfWork unitOfWork) : IShopService
{
    public async Task<Shop> CreateAsync(string name, string address, CancellationToken cancellationToken = default)
    {
        var shop = new Shop
        {
            Id = Guid.NewGuid(),
            Name = name,
            Address = address,
            CreatedAt = DateTime.UtcNow
        };

        await shopRepository.AddAsync(shop, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return shop;
    }

    public async Task<Shop?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await shopRepository.GetByIdAsync(id, cancellationToken);
    }
}