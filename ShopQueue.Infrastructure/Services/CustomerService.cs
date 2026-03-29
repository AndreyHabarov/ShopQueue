using MassTransit;
using Microsoft.EntityFrameworkCore;
using ShopQueue.Application.Exceptions;
using ShopQueue.Application.Messages;
using ShopQueue.Application.Services;
using ShopQueue.Domain.Entities;
using ShopQueue.Domain.Enums;
using ShopQueue.Infrastructure.Persistence;

namespace ShopQueue.Infrastructure.Services;

public class CustomerService(AppDbContext db, IPublishEndpoint publishEndpoint) : ICustomerService
{
    public async Task<QueueEntry> JoinQueueAsync(Guid queueId, string customerName, string customerPhone)
    {
        var queue = await db.Queues.FindAsync(queueId);
        if (queue is null)
        {
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
            .CountAsync() + 1;

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
        await db.SaveChangesAsync();

        await publishEndpoint.Publish(new ClientJoinedQueue(
            entry.Id,
            entry.QueueId,
            entry.CustomerId,
            entry.Position,
            entry.JoinedAt
        ));

        return entry;
    }
}