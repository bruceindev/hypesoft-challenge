using MediatR;
using Hypesoft.Application.Products.DTOs;

namespace Hypesoft.Application.Products.Queries.GetLowStockProducts;

public class GetLowStockProductsQuery : IRequest<IReadOnlyList<ProductResponseDto>>
{
    public int Threshold { get; set; } = 10;
}
