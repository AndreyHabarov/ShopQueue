using ShopQueue.Domain.Entities;

namespace ShopQueue.Application.Services;

public interface IShopService
{
    Task<Shop> CreateAsync(string name, string address, CancellationToken cancellationToken = default);
}