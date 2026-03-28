using Microsoft.EntityFrameworkCore;
using ShopQueue.Application.Services;
using ShopQueue.Domain.Entities;
using ShopQueue.Domain.Enums;
using ShopQueue.Infrastructure.Persistence;

namespace ShopQueue.Infrastructure.Services;

public class QueueService(AppDbContext db) : IQueueService
{
    public async Task<Queue> CreateAsync(Guid shopId, string name)
    {
        var queue = new Queue
        {
            Id = Guid.NewGuid(),
            ShopId = shopId,
            Name = name,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        db.Queues.Add(queue);
        await db.SaveChangesAsync();
        return queue;
    }

    public async Task<List<QueueEntry>> GetEntriesAsync(Guid queueId)
    {
        return await db.QueueEntries
            .Include(e => e.Customer)
            .Where(e => e.QueueId == queueId && e.Status == QueueEntryStatus.Waiting)
            .OrderBy(e => e.Position)
            .ToListAsync();
    }

    public async Task<QueueEntry> CallNextAsync(Guid queueId)
    {
        var next = await db.QueueEntries
            .Where(e => e.QueueId == queueId && e.Status == QueueEntryStatus.Waiting)
            .OrderBy(e => e.Position)
            .FirstOrDefaultAsync();

        if (next is null)
            throw new InvalidOperationException("Queue is empty");

        next.Status = QueueEntryStatus.Called;
        next.CalledAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return next;
    }
}