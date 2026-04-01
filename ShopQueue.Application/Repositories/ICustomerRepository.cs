using ShopQueue.Domain.Entities;

namespace ShopQueue.Application.Repositories;

public interface ICustomerRepository
{
    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);
}
