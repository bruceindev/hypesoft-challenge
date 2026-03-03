using MediatR;
using Hypesoft.Application.Products.DTOs;

namespace Hypesoft.Application.Products.Commands.UpdateProduct;

public class UpdateProductCommand : IRequest<ProductResponseDto>
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string CategoryId { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}
