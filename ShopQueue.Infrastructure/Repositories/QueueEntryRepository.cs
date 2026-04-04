using Microsoft.EntityFrameworkCore;
using ShopQueue.Application.Repositories;
using ShopQueue.Domain.Entities;
using ShopQueue.Domain.Enums;
using ShopQueue.Infrastructure.Persistence;

namespace ShopQueue.Infrastructure.Repositories;

public class QueueEntryRepository(AppDbContext db) : IQueueEntryRepository
{
    public async Task<int> CountWaitingAsync(Guid queueId, CancellationToken cancellationToken = default)
    {
        return await db.QueueEntries
            .Where(e => e.QueueId == queueId && e.Status == QueueEntryStatus.Waiting)
            .CountAsync(cancellationToken);
    }

    public async Task<List<QueueEntry>> GetWaitingAsync(Guid queueId, CancellationToken cancellationToken = default)
    {
        return await db.QueueEntries
            .Include(e => e.Customer)
            .Where(e => e.QueueId == queueId && e.Status == QueueEntryStatus.Waiting)
            .OrderBy(e => e.Position)
            .ToListAsync(cancellationToken);
    }

    public async Task<QueueEntry?> GetNextWaitingAsync(Guid queueId, CancellationToken cancellationToken = default)
    {
        return await db.QueueEntries
            .Where(e => e.QueueId == queueId && e.Status == QueueEntryStatus.Waiting)
            .OrderBy(e => e.Position)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(QueueEntry entry, CancellationToken cancellationToken = default)
    {
        await db.QueueEntries.AddAsync(entry, cancellationToken);
    }

    public async Task<QueueEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await db.QueueEntries
            .Include(e => e.Customer)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }
}