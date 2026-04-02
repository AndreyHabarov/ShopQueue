using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using ShopQueue.Application.Exceptions;
using ShopQueue.Application.Repositories;
using ShopQueue.Domain.Entities;
using ShopQueue.Infrastructure.Services;
using Xunit;

namespace ShopQueue.Tests.Services;

public class QueueServiceTests
{
    private readonly Mock<ILogger<QueueService>> _loggerMock = new();
    private readonly Mock<IPublishEndpoint> _publishEndpointMock = new();
    private readonly Mock<IQueueEntryRepository> _queueEntryRepositoryMock = new();
    private readonly Mock<IQueueRepository> _queueRepositoryMock = new();
    private readonly Mock<IShopRepository> _shopRepositoryMock = new();

    private readonly QueueService _sut;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public QueueServiceTests()
    {
        _sut = new QueueService(
            _shopRepositoryMock.Object,
            _queueRepositoryMock.Object,
            _queueEntryRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _publishEndpointMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task CallNextAsync_ShouldThrowNotFoundException_WhenQueueNotFound()
    {
        //Arrange
        var queueId = Guid.NewGuid();

        _queueRepositoryMock
            .Setup(x => x.GetByIdAsync(queueId, default))
            .ReturnsAsync((Queue?)null);

        //Act
        var act = async () => await _sut.CallNextAsync(queueId);

        //Assert
        await Assert.ThrowsAsync<NotFoundException>(act);
    }
}