using AutoMapper;
using FluentAssertions;
using Hypesoft.Application.Categories.DTOs;
using Hypesoft.Application.Categories.Queries.GetCategories;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Interfaces;
using Moq;

namespace Hypesoft.Tests.Application;

public class GetCategoriesHandlerCacheTests
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock = new();
    private readonly Mock<ICacheService> _cacheServiceMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    [Fact]
    public async Task Handle_ShouldReturnCachedValue_WhenCacheHit()
    {
        var cached = new List<CategoryResponseDto>
        {
            new() { Id = "cat-1", Name = "Eletrônicos", Description = "Desc" }
        };

        _cacheServiceMock
            .Setup(cache => cache.GetAsync<List<CategoryResponseDto>>("categories:list", It.IsAny<CancellationToken>()))
            .ReturnsAsync(cached);

        var handler = new GetCategoriesHandler(_categoryRepositoryMock.Object, _cacheServiceMock.Object, _mapperMock.Object);

        var result = await handler.Handle(new GetCategoriesQuery(), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Eletrônicos");
        _categoryRepositoryMock.Verify(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldLoadRepositoryAndCache_WhenCacheMiss()
    {
        var categories = new List<Category>
        {
            Category.Create("Eletrônicos", "Desc")
        };

        var mapped = new List<CategoryResponseDto>
        {
            new() { Id = categories[0].Id, Name = categories[0].Name, Description = categories[0].Description }
        };

        _cacheServiceMock
            .Setup(cache => cache.GetAsync<List<CategoryResponseDto>>("categories:list", It.IsAny<CancellationToken>()))
            .ReturnsAsync((List<CategoryResponseDto>?)null);

        _categoryRepositoryMock
            .Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        _mapperMock
            .Setup(mapper => mapper.Map<List<CategoryResponseDto>>(It.IsAny<List<Category>>()))
            .Returns(mapped);

        var handler = new GetCategoriesHandler(_categoryRepositoryMock.Object, _cacheServiceMock.Object, _mapperMock.Object);

        var result = await handler.Handle(new GetCategoriesQuery(), CancellationToken.None);

        result.Should().HaveCount(1);

        _cacheServiceMock.Verify(
            cache => cache.SetAsync("categories:list", It.IsAny<List<CategoryResponseDto>>(), It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
