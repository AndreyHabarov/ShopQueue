using ShopQueue.Application.Repositories;
using ShopQueue.Infrastructure.Persistence;

namespace ShopQueue.Infrastructure.Repositories;

public class UnitOfWork(AppDbContext db) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await db.SaveChangesAsync(cancellationToken);
    }
}