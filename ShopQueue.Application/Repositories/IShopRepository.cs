using ShopQueue.Domain.Entities;

namespace ShopQueue.Application.Repositories;

public interface IShopRepository
{
    Task<Shop?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Shop shop, CancellationToken cancellationToken = default);
}