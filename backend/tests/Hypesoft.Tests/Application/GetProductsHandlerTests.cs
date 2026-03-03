using AutoMapper;
using FluentAssertions;
using Hypesoft.Application.Products.DTOs;
using Hypesoft.Application.Products.Queries.GetProducts;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Interfaces;
using Moq;

namespace Hypesoft.Tests.Application;

public class GetProductsHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    [Fact]
    public async Task Handle_ShouldCallRepositoryWithProvidedFilterAndPagination()
    {
        var handler = new GetProductsHandler(_productRepositoryMock.Object, _mapperMock.Object);
        var query = new GetProductsQuery
        {
            PageNumber = 2,
            PageSize = 5,
            SearchTerm = "notebook",
            CategoryId = "cat-1"
        };

        _productRepositoryMock
            .Setup(repository => repository.GetPagedAsync(2, 5, "notebook", "cat-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Product>(), 0));

        _mapperMock
            .Setup(mapper => mapper.Map<List<ProductResponseDto>>(It.IsAny<IReadOnlyList<Product>>()))
            .Returns([]);

        await handler.Handle(query, CancellationToken.None);

        _productRepositoryMock.Verify(
            repository => repository.GetPagedAsync(2, 5, "notebook", "cat-1", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedItemsAndPaginationMetadata()
    {
        var product = Product.Create("Produto 1", "Desc", 30, 12, "cat-1");
        var mappedItems = new List<ProductResponseDto>
        {
            new() { Id = product.Id, Name = product.Name, CategoryId = product.CategoryId, StockQuantity = product.StockQuantity.Value, Price = product.Price.Amount }
        };

        var handler = new GetProductsHandler(_productRepositoryMock.Object, _mapperMock.Object);

        _productRepositoryMock
            .Setup(repository => repository.GetPagedAsync(1, 10, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Product> { product }, 15));

        _mapperMock
            .Setup(mapper => mapper.Map<List<ProductResponseDto>>(It.IsAny<IReadOnlyList<Product>>()))
            .Returns(mappedItems);

        var result = await handler.Handle(new GetProductsQuery { PageNumber = 1, PageSize = 10 }, CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].Name.Should().Be("Produto 1");
        result.TotalCount.Should().Be(15);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenNoItems_ShouldReturnEmptyResult()
    {
        var handler = new GetProductsHandler(_productRepositoryMock.Object, _mapperMock.Object);

        _productRepositoryMock
            .Setup(repository => repository.GetPagedAsync(1, 10, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Product>(), 0));

        _mapperMock
            .Setup(mapper => mapper.Map<List<ProductResponseDto>>(It.IsAny<IReadOnlyList<Product>>()))
            .Returns([]);

        var result = await handler.Handle(new GetProductsQuery(), CancellationToken.None);

        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.HasNextPage.Should().BeFalse();
    }
}
