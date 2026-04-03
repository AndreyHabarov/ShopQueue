using Moq;
using ShopQueue.Application.Repositories;
using ShopQueue.Domain.Entities;
using ShopQueue.Infrastructure.Services;
using Xunit;

namespace ShopQueue.Tests.Services;

public class ShopServiceTests
{
    private readonly Mock<IShopRepository> _shopRepositoryMock = new();

    private readonly ShopService _sut;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public ShopServiceTests()
    {
        _sut = new ShopService(
            _shopRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnShop_WhenCalled()
    {
        //Arrange
        var name = "Specific shop";
        var address = "Certain street 25";

        //Act
        var result = await _sut.CreateAsync(name, address);

        //Assert
        Assert.Equal(name, result.Name);
        Assert.Equal(address, result.Address);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        _shopRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Shop>(), default), Times.Once);
    }
}