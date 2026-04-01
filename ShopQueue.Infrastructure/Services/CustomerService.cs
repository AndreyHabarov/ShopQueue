using MassTransit;
using Microsoft.Extensions.Logging;
using ShopQueue.Application.Exceptions;
using ShopQueue.Application.Messages;
using ShopQueue.Application.Repositories;
using ShopQueue.Application.Services;
using ShopQueue.Domain.Entities;
using ShopQueue.Domain.Enums;

namespace ShopQueue.Infrastructure.Services;

public class CustomerService(
    ICustomerRepository customerRepository,
    IQueueRepository queueRepository,
    IQueueEntryRepository queueEntryRepository,
    IUnitOfWork unitOfWork,
    IPublishEndpoint publishEndpoint,
    ILogger<CustomerService> logger) : ICustomerService
{
    public async Task<QueueEntry> JoinQueueAsync(Guid queueId, string customerName, string customerPhone,
        CancellationToken cancellationToken = default)
    {
        var queue = await queueRepository.GetByIdAsync(queueId, cancellationToken);
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

        await customerRepository.AddAsync(customer, cancellationToken);

        var position = await queueEntryRepository.CountWaitingAsync(queueId, cancellationToken) + 1;

        var entry = new QueueEntry
        {
            Id = Guid.NewGuid(),
            QueueId = queueId,
            CustomerId = customer.Id,
            Position = position,
            Status = QueueEntryStatus.Waiting,
            JoinedAt = DateTime.UtcNow
        };

        await queueEntryRepository.AddAsync(entry, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await publishEndpoint.Publish(new ClientJoinedQueue(
            entry.Id, entry.QueueId, entry.CustomerId, entry.Position, entry.JoinedAt), cancellationToken);

        logger.LogInformation("Customer joined queue. EntryId={EntryId}, QueueId={QueueId}, Position={Position}",
            entry.Id, queueId, entry.Position);

        return entry;
    }
}
