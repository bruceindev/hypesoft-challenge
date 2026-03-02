using MediatR;
using Hypesoft.Domain.Interfaces;
using Hypesoft.Domain.ValueObjects;

namespace Hypesoft.Application.Dashboard.Queries.GetDashboardStats;

public class GetDashboardStatsHandler : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public GetDashboardStatsHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<DashboardStatsDto> Handle(
        GetDashboardStatsQuery request,
        CancellationToken cancellationToken)
    {
        var allProducts = await _productRepository.GetAllAsync(cancellationToken);
        var allCategories = await _categoryRepository.GetAllAsync(cancellationToken);
        var lowStockProducts = await _productRepository.GetLowStockProductsAsync(StockQuantity.LowStockThreshold, cancellationToken);

        var totalProducts = allProducts.Count();
        var totalStockValue = allProducts.Sum(p => p.Price.Amount * p.StockQuantity.Value);
        var lowStockCount = lowStockProducts.Count();

        var productsByCategory = allProducts
            .GroupBy(p => p.CategoryId)
            .Select(g => new CategoryStatsDto
            {
                CategoryId = g.Key,
                Name = allCategories.FirstOrDefault(c => c.Id == g.Key)?.Name ?? "Unknown",
                Total = g.Count()
            })
            .ToList();

        var lowStockItems = lowStockProducts
            .Select(product => new DashboardLowStockProductDto
            {
                Id = product.Id,
                Name = product.Name,
                CategoryId = product.CategoryId,
                CategoryName = allCategories.FirstOrDefault(category => category.Id == product.CategoryId)?.Name,
                Price = product.Price.Amount,
                Stock = product.StockQuantity.Value
            })
            .ToList();

        return new DashboardStatsDto
        {
            TotalProducts = totalProducts,
            TotalStockValue = totalStockValue,
            LowStockCount = lowStockCount,
            LowStockProducts = lowStockItems,
            ProductsByCategory = productsByCategory
        };
    }
}
