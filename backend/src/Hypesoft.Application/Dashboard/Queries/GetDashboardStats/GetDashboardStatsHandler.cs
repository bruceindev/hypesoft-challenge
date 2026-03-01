using MediatR;
using Hypesoft.Application.Common.Models;
using Hypesoft.Domain.Interfaces;

namespace Hypesoft.Application.Dashboard.Queries.GetDashboardStats;

public class GetDashboardStatsHandler : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
{
    private readonly IProductRepository _productRepository;

    public GetDashboardStatsHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<DashboardStatsDto> Handle(
        GetDashboardStatsQuery request,
        CancellationToken cancellationToken)
    {
        var totalProducts = await _productRepository.CountAsync(null, null, cancellationToken);
        var totalStockValue = await _productRepository.GetTotalStockValueAsync(cancellationToken);
        var lowStockProducts = await _productRepository.GetLowStockAsync(10, cancellationToken);

        return new DashboardStatsDto
        {
            TotalProducts = totalProducts,
            TotalStockValue = totalStockValue,
            LowStockProductsCount = lowStockProducts.Count
        };
    }
}
