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

public class CustomerService(AppDbContext db, IPublishEndpoint publishEndpoint, ILogger<CustomerService> logger)
    : ICustomerService
{
    public async Task<QueueEntry> JoinQueueAsync(Guid queueId, string customerName, string customerPhone,
        CancellationToken cancellationToken = default)
    {
        var queue = await db.Queues.FindAsync([queueId], cancellationToken);
        if (queue is null)
        {
            logger.LogWarning("Queue not found. QueueId={QueueId}", queueId);
            throw new NotFoundException($"Queue with id {queueId} not found");
        }

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Name = customerName,
            Phone = customerPhone
        };

        db.Customers.Add(customer);

        var position = await db.QueueEntries
            .Where(e => e.QueueId == queueId && e.Status == QueueEntryStatus.Waiting)
            .CountAsync(cancellationToken) + 1;

        var entry = new QueueEntry
        {
            Id = Guid.NewGuid(),
            QueueId = queueId,
            CustomerId = customer.Id,
            Position = position,
            Status = QueueEntryStatus.Waiting,
            JoinedAt = DateTime.UtcNow
        };

        db.QueueEntries.Add(entry);
        await db.SaveChangesAsync(cancellationToken);

        await publishEndpoint.Publish(new ClientJoinedQueue(
            entry.Id,
            entry.QueueId,
            entry.CustomerId,
            entry.Position,
            entry.JoinedAt
        ));

        logger.LogInformation("Customer joined queue. EntryId = {EntryId}, QueueId = {QueueId}, Position = {Position}",
            entry.Id, queueId, entry.Position);

        return entry;
    }
}