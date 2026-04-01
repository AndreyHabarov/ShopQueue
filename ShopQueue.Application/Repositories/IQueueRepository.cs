using ShopQueue.Domain.Entities;

namespace ShopQueue.Application.Repositories;

public interface IQueueRepository
{
    Task<Queue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Queue queue, CancellationToken cancellationToken = default);
}
