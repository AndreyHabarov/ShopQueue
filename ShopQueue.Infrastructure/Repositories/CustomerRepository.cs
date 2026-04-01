using ShopQueue.Application.Repositories;
using ShopQueue.Domain.Entities;
using ShopQueue.Infrastructure.Persistence;

namespace ShopQueue.Infrastructure.Repositories;

public class CustomerRepository(AppDbContext db) : ICustomerRepository
{
    public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        await db.Customers.AddAsync(customer, cancellationToken);
    }
}
