using FluentAssertions;
using Hypesoft.Application.Dashboard.Queries.GetDashboardStats;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Interfaces;
using Moq;

namespace Hypesoft.Tests.Application;

public class DashboardCacheTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock = new();
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock = new();
    private readonly Mock<ICacheService> _cacheServiceMock = new();

    [Fact]
    public async Task Handle_ShouldReturnCachedStats_WhenCacheHit()
    {
        var cached = new DashboardStatsDto
        {
            TotalProducts = 10,
            TotalStockValue = 1000,
            LowStockCount = 2,
            ProductsByCategory = new List<CategoryStatsDto>(),
            LowStockProducts = new List<DashboardLowStockProductDto>()
        };

        _cacheServiceMock
            .Setup(cache => cache.GetAsync<DashboardStatsDto>("dashboard:stats", It.IsAny<CancellationToken>()))
            .ReturnsAsync(cached);

        var handler = new GetDashboardStatsHandler(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _cacheServiceMock.Object);

        var result = await handler.Handle(new GetDashboardStatsQuery(), CancellationToken.None);

        result.TotalProducts.Should().Be(10);

        _productRepositoryMock.Verify(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()), Times.Never);
        _categoryRepositoryMock.Verify(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldCacheCalculatedStats_WhenCacheMiss()
    {
        _cacheServiceMock
            .Setup(cache => cache.GetAsync<DashboardStatsDto>("dashboard:stats", It.IsAny<CancellationToken>()))
            .ReturnsAsync((DashboardStatsDto?)null);

        var category = Category.Create("Eletrônicos", "Desc");
        var product = Product.Create("Produto", "Produto descrição", 50, 8, category.Id);

        _productRepositoryMock
            .Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product> { product });

        _productRepositoryMock
            .Setup(repository => repository.GetLowStockProductsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product> { product });

        _categoryRepositoryMock
            .Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Category> { category });

        var handler = new GetDashboardStatsHandler(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _cacheServiceMock.Object);

        var result = await handler.Handle(new GetDashboardStatsQuery(), CancellationToken.None);

        result.TotalProducts.Should().Be(1);
        result.LowStockCount.Should().Be(1);

        _cacheServiceMock.Verify(
            cache => cache.SetAsync("dashboard:stats", It.IsAny<DashboardStatsDto>(), It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
