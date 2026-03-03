using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace Hypesoft.Tests.Integration;

public class ProductFlowIntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _httpClient;

    public ProductFlowIntegrationTests(TestWebApplicationFactory factory)
    {
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task CreateCategory_ThenCreateProduct_ThenGetPaginatedProducts_ShouldSucceed()
    {
        var categoryResponse = await _httpClient.PostAsJsonAsync("/api/categories", new
        {
            name = "Categoria integração",
            description = "Descrição categoria integração"
        });

        categoryResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var category = await categoryResponse.Content.ReadFromJsonAsync<CategoryResponse>();
        category.Should().NotBeNull();

        var productResponse = await _httpClient.PostAsJsonAsync("/api/products", new
        {
            name = "Produto integração",
            description = "Descrição produto integração",
            price = 120,
            stockQuantity = 6,
            categoryId = category!.Id
        });

        productResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var pagedResponse = await _httpClient.GetAsync("/api/products?pageNumber=1&pageSize=1");
        pagedResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var paged = await pagedResponse.Content.ReadFromJsonAsync<PagedProductResponse>();
        paged.Should().NotBeNull();
        paged!.PageNumber.Should().Be(1);
        paged.PageSize.Should().Be(1);
        paged.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateProducts_ThenFilterBySearchAndCategory_ShouldReturnExpectedResult()
    {
        var categoryResponse = await _httpClient.PostAsJsonAsync("/api/categories", new
        {
            name = "Categoria filtro",
            description = "Descrição categoria filtro"
        });

        var category = await categoryResponse.Content.ReadFromJsonAsync<CategoryResponse>();
        category.Should().NotBeNull();

        await _httpClient.PostAsJsonAsync("/api/products", new
        {
            name = "Notebook Alpha",
            description = "Produto 1",
            price = 5000,
            stockQuantity = 20,
            categoryId = category!.Id
        });

        await _httpClient.PostAsJsonAsync("/api/products", new
        {
            name = "Mouse Beta",
            description = "Produto 2",
            price = 100,
            stockQuantity = 30,
            categoryId = category.Id
        });

        var filteredResponse = await _httpClient.GetAsync($"/api/products?pageNumber=1&pageSize=10&searchTerm=Notebook&categoryId={category.Id}");

        filteredResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var paged = await filteredResponse.Content.ReadFromJsonAsync<PagedProductResponse>();
        paged.Should().NotBeNull();
        paged!.Items.Should().ContainSingle(item => item.Name.Contains("Notebook"));
    }

    private sealed class CategoryResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    private sealed class PagedProductResponse
    {
        public List<ProductResponse> Items { get; set; } = [];
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    private sealed class ProductResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
