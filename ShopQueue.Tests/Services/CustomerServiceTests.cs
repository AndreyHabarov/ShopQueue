using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using ShopQueue.Application.Exceptions;
using ShopQueue.Application.Repositories;
using ShopQueue.Domain.Entities;
using ShopQueue.Domain.Enums;
using ShopQueue.Infrastructure.Services;
using Xunit;

namespace ShopQueue.Tests.Services;

public class CustomerServiceTests
{
    private readonly Mock<ICustomerRepository> _customerRepositoryMock = new();
    private readonly Mock<ILogger<CustomerService>> _loggerMock = new();
    private readonly Mock<IPublishEndpoint> _publishEndpointMock = new();
    private readonly Mock<IQueueEntryRepository> _queueEntryRepositoryMock = new();
    private readonly Mock<IQueueRepository> _queueRepositoryMock = new();

    private readonly CustomerService _sut;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    private const string CustomerName = "Eugene";
    private const string CustomerPhone = "72323232";

    public CustomerServiceTests()
    {
        _sut = new CustomerService(
            _customerRepositoryMock.Object,
            _queueRepositoryMock.Object,
            _queueEntryRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _publishEndpointMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task JoinQueueAsync_ShouldThrowNotFoundException_WhenQueueNotFound()
    {
        //Arrange
        var queueId = Guid.NewGuid();
        _queueRepositoryMock
            .Setup(x => x.GetByIdAsync(queueId, default))
            .ReturnsAsync((Queue?)null);

        //Act
        var act = async () => await _sut.JoinQueueAsync(queueId, CustomerName, CustomerPhone);

        //Assert
        await Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Fact]
    public async Task JoinQueueAsync_ShouldReturnEntryWithCorrectPosition_WhenQueueExists()
    {
        //Arrange
        var queueId = Guid.NewGuid();

        _queueRepositoryMock
            .Setup(x => x.GetByIdAsync(queueId, default))
            .ReturnsAsync(new Queue {Id = queueId});

        _queueEntryRepositoryMock
            .Setup(x => x.CountWaitingAsync(queueId, default))
            .ReturnsAsync(3);

        //Act
        var result = await _sut.JoinQueueAsync(queueId, CustomerName, CustomerPhone);

        //Assert
        Assert.Equal(4, result.Position);
        Assert.Equal(QueueEntryStatus.Waiting, result.Status);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }
}