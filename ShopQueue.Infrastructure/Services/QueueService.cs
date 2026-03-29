using MassTransit;
using Microsoft.EntityFrameworkCore;
using ShopQueue.Application.Exceptions;
using ShopQueue.Application.Messages;
using ShopQueue.Application.Services;
using ShopQueue.Domain.Entities;
using ShopQueue.Domain.Enums;
using ShopQueue.Infrastructure.Persistence;

namespace ShopQueue.Infrastructure.Services;

public class QueueService(AppDbContext db, IPublishEndpoint publishEndpoint) : IQueueService
{
    public async Task<Queue> CreateAsync(Guid shopId, string name)
    {
        var shop = await db.Shops.FindAsync(shopId);
        if (shop is null)
        {
            throw new NotFoundException($"Shop with id {shopId} not found");
        }

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
        var queue = await db.Queues.FindAsync(queueId);
        if (queue is null)
        {
            throw new NotFoundException($"Queue with id {queueId} not found");
        }

        return await db.QueueEntries
            .Include(e => e.Customer)
            .Where(e => e.QueueId == queueId && e.Status == QueueEntryStatus.Waiting)
            .OrderBy(e => e.Position)
            .ToListAsync();
    }

    public async Task<QueueEntry> CallNextAsync(Guid queueId)
    {
        var queue = await db.Queues.FindAsync(queueId);
        if (queue is null)
        {
            throw new NotFoundException($"Queue with id {queueId} not found");
        }
        
        var next = await db.QueueEntries
            .Where(e => e.QueueId == queueId && e.Status == QueueEntryStatus.Waiting)
            .OrderBy(e => e.Position)
            .FirstOrDefaultAsync();

        if (next is null)
            throw new BusinessException("Queue is empty");

        next.Status = QueueEntryStatus.Called;
        next.CalledAt = DateTime.UtcNow;

        await db.SaveChangesAsync();

        await publishEndpoint.Publish(new ClientCalled(
            next.Id,
            next.QueueId,
            next.CustomerId,
            next.Position,
            next.CalledAt!.Value
        ));

        return next;
    }
}