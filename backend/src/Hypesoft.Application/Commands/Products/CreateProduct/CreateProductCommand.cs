using MediatR;

namespace Hypesoft.Application.Commands.Products.CreateProduct;

public class CreateProductCommand : IRequest<Guid>
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public Guid CategoryId { get; init; }
    public int StockQuantity { get; init; }
}