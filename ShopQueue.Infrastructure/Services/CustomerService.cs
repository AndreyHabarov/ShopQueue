using Microsoft.EntityFrameworkCore;
using ShopQueue.Application.Services;
using ShopQueue.Domain.Entities;
using ShopQueue.Domain.Enums;
using ShopQueue.Infrastructure.Persistence;

namespace ShopQueue.Infrastructure.Services;

public class CustomerService(AppDbContext db) : ICustomerService
{
    public async Task<QueueEntry> JoinQueueAsync(Guid queueId, string customerName, string customerPhone)
    {
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Name = customerName,
            Phone = customerPhone
        };

        db.Customers.Add(customer);

        var postion = await db.QueueEntries
            .Where(e => e.QueueId == queueId && e.Status == QueueEntryStatus.Waiting)
            .CountAsync() + 1;

        var entry = new QueueEntry
        {
            Id = Guid.NewGuid(),
            QueueId = queueId,
            CustomerId = customer.Id,
            Position = postion,
            Status = QueueEntryStatus.Waiting,
            JoinedAt = DateTime.UtcNow
        };

        db.QueueEntries.Add(entry);
        await db.SaveChangesAsync();
        return entry;
    }
}