using MediatR;
using Hypesoft.Application.Products.DTOs;

namespace Hypesoft.Application.Products.Queries.GetLowStockProducts;

public class GetLowStockProductsQuery : IRequest<List<ProductResponseDto>>
{
    public int Threshold { get; set; } = 10;
}
