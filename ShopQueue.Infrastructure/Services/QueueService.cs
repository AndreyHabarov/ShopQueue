using MassTransit;
using Microsoft.Extensions.Logging;
using ShopQueue.Application.Exceptions;
using ShopQueue.Application.Messages;
using ShopQueue.Application.Repositories;
using ShopQueue.Application.Services;
using ShopQueue.Domain.Entities;
using ShopQueue.Domain.Enums;

namespace ShopQueue.Infrastructure.Services;

public class QueueService(
    IShopRepository shopRepository,
    IQueueRepository queueRepository,
    IQueueEntryRepository queueEntryRepository,
    IUnitOfWork unitOfWork,
    IPublishEndpoint publishEndpoint,
    ILogger<QueueService> logger) : IQueueService
{
    public async Task<Queue> CreateAsync(Guid shopId, string name, CancellationToken cancellationToken = default)
    {
        var shop = await shopRepository.GetByIdAsync(shopId, cancellationToken);
        if (shop is null)
        {
            logger.LogWarning("Shop not found. ShopId={ShopId}", shopId);
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

        await queueRepository.AddAsync(queue, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Queue created. QueueId={QueueId}, ShopId={ShopId}, Name={Name}", queue.Id, shopId, name);
        return queue;
    }

    public async Task<List<QueueEntry>> GetEntriesAsync(Guid queueId, CancellationToken cancellationToken = default)
    {
        var queue = await queueRepository.GetByIdAsync(queueId, cancellationToken);
        if (queue is null)
            throw new NotFoundException($"Queue with id {queueId} not found");

        return await queueEntryRepository.GetWaitingAsync(queueId, cancellationToken);
    }

    public async Task<QueueEntry> CallNextAsync(Guid queueId, CancellationToken cancellationToken = default)
    {
        var queue = await queueRepository.GetByIdAsync(queueId, cancellationToken);
        if (queue is null)
            throw new NotFoundException($"Queue with id {queueId} not found");

        var next = await queueEntryRepository.GetNextWaitingAsync(queueId, cancellationToken);
        if (next is null)
        {
            logger.LogWarning("Call next failed - queue is empty. QueueId={QueueId}", queueId);
            throw new BusinessException("Queue is empty");
        }

        next.Status = QueueEntryStatus.Called;
        next.CalledAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await publishEndpoint.Publish(new ClientCalled(
            next.Id, next.QueueId, next.CustomerId, next.Position, next.CalledAt!.Value), cancellationToken);

        logger.LogInformation("Customer called. EntryId={EntryId}, QueueId={QueueId}, Position={Position}",
            next.Id, queueId, next.Position);

        return next;
    }

    public async Task<List<Queue>> GetByShopIdAsync(Guid shopId, CancellationToken cancellationToken = default)
    {
        var shop = await shopRepository.GetByIdAsync(shopId, cancellationToken);
        if (shop is null)
            throw new NotFoundException($"Shop with id {shopId} not found");

        return await queueRepository.GetByShopIdAsync(shopId, cancellationToken);
    }

    public async Task<QueueEntry> MarkAsServedAsync(Guid queueId, Guid entryId,
        CancellationToken cancellationToken = default)
    {
        var queue = await queueRepository.GetByIdAsync(queueId, cancellationToken);
        if (queue is null)
            throw new NotFoundException($"Queue with id {queueId} not found");

        var entry = await queueEntryRepository.GetByIdAsync(entryId, cancellationToken);
        if (entry is null)
            throw new NotFoundException($"Entry with id {entryId} not found");

        if (entry.Status != QueueEntryStatus.Called)
            throw new BusinessException("Only called entries can be marked as served");

        entry.Status = QueueEntryStatus.Served;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return entry;
    }
}