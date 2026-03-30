using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShopQueue.Application.Exceptions;
using ShopQueue.Application.Messages;
using ShopQueue.Application.Services;
using ShopQueue.Domain.Entities;
using ShopQueue.Domain.Enums;
using ShopQueue.Infrastructure.Persistence;

namespace ShopQueue.Infrastructure.Services;

public class QueueService(AppDbContext db, IPublishEndpoint publishEndpoint, ILogger<QueueService> logger)
    : IQueueService
{
    public async Task<Queue> CreateAsync(Guid shopId, string name, CancellationToken cancellationToken = default)
    {
        var shop = await db.Shops.FindAsync([shopId], cancellationToken);
        if (shop is null)
        {
            logger.LogWarning("Shop not found. ShopId = {ShopId}", shopId);
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
        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Queue created. QueueId={QueueId}, ShopId={ShopId}, Name={Name}", queue.Id, shopId, name);
        return queue;
    }

    public async Task<List<QueueEntry>> GetEntriesAsync(Guid queueId, CancellationToken cancellationToken = default)
    {
        var queue = await db.Queues.FindAsync([queueId], cancellationToken);
        if (queue is null)
        {
            throw new NotFoundException($"Queue with id {queueId} not found");
        }

        return await db.QueueEntries
            .Include(e => e.Customer)
            .Where(e => e.QueueId == queueId && e.Status == QueueEntryStatus.Waiting)
            .OrderBy(e => e.Position)
            .ToListAsync(cancellationToken);
    }

    public async Task<QueueEntry> CallNextAsync(Guid queueId, CancellationToken cancellationToken = default)
    {
        var queue = await db.Queues.FindAsync([queueId], cancellationToken);
        if (queue is null)
        {
            throw new NotFoundException($"Queue with id {queueId} not found");
        }

        var next = await db.QueueEntries
            .Where(e => e.QueueId == queueId && e.Status == QueueEntryStatus.Waiting)
            .OrderBy(e => e.Position)
            .FirstOrDefaultAsync(cancellationToken);

        if (next is null)
        {
            logger.LogWarning("Call next failed - queue is empty. QueueId = {QueueId}", queueId);
            throw new BusinessException("Queue is empty");
        }

        next.Status = QueueEntryStatus.Called;
        next.CalledAt = DateTime.UtcNow;

        await db.SaveChangesAsync(cancellationToken);

        await publishEndpoint.Publish(new ClientCalled(
            next.Id,
            next.QueueId,
            next.CustomerId,
            next.Position,
            next.CalledAt!.Value
        ));

        logger.LogInformation("Customer called. EntryId = {EntryId}, QueueId = {QueueId}, Position = {Position}",
            next.Id, queueId, next.Position);

        return next;
    }
}