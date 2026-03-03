using MediatR;
using Hypesoft.Application.Products.DTOs;

namespace Hypesoft.Application.Products.Commands.CreateProduct;

public class CreateProductCommand : IRequest<ProductResponseDto>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string CategoryId { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public string? ImageUrl { get; set; }
}
