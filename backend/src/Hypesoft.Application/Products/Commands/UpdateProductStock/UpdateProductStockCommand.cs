using MediatR;
using Hypesoft.Application.Products.DTOs;

namespace Hypesoft.Application.Products.Commands.UpdateProductStock;

public class UpdateProductStockCommand : IRequest<ProductResponseDto>
{
    public string Id { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
}
