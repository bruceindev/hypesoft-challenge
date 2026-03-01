using MediatR;
using Hypesoft.Application.Common.Models;
using Hypesoft.Application.Products.DTOs;

namespace Hypesoft.Application.Products.Queries.GetProducts;

public class GetProductsQuery : IRequest<PagedResponse<ProductResponseDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public Guid? CategoryId { get; set; }
}
