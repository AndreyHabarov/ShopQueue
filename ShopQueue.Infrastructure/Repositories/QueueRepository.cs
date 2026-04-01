using ShopQueue.Application.Repositories;
using ShopQueue.Domain.Entities;
using ShopQueue.Infrastructure.Persistence;

namespace ShopQueue.Infrastructure.Repositories;

public class QueueRepository(AppDbContext db) : IQueueRepository
{
    public async Task<Queue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await db.Queues.FindAsync([id], cancellationToken);
    }

    public async Task AddAsync(Queue queue, CancellationToken cancellationToken = default)
    {
        await db.Queues.AddAsync(queue, cancellationToken);
    }
}
