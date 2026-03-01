using MediatR;
using Hypesoft.Application.Products.DTOs;

namespace Hypesoft.Application.Products.Commands.UpdateProductStock;

public class UpdateProductStockCommand : IRequest<ProductResponseDto>
{
    public Guid Id { get; set; }
    public int StockQuantity { get; set; }
}
