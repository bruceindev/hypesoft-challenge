using MediatR;

namespace Hypesoft.Application.Dashboard.Queries.GetDashboardStats;

public class GetDashboardStatsQuery : IRequest<DashboardStatsDto>
{
}

public class DashboardStatsDto
{
    public int TotalProducts { get; set; }
    public decimal TotalStockValue { get; set; }
    public int LowStockCount { get; set; }
    public List<DashboardLowStockProductDto> LowStockProducts { get; set; } = new();
    public List<CategoryStatsDto> ProductsByCategory { get; set; } = new();
}

public class CategoryStatsDto
{
    public string CategoryId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Total { get; set; }
}

public class DashboardLowStockProductDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string CategoryId { get; set; } = string.Empty;
    public string? CategoryName { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
}
