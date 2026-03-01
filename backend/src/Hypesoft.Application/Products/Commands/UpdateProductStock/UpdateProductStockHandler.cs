using MediatR;
using Hypesoft.Application.Products.DTOs;
using Hypesoft.Domain.Interfaces;

namespace Hypesoft.Application.Products.Commands.UpdateProductStock;

public class UpdateProductStockHandler : IRequestHandler<UpdateProductStockCommand, ProductResponseDto>
{
    private readonly IProductRepository _productRepository;

    public UpdateProductStockHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductResponseDto> Handle(
        UpdateProductStockCommand request,
        CancellationToken cancellationToken)
    {
        if (request.StockQuantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative.");

        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

        if (product == null)
            throw new KeyNotFoundException($"Product with ID {request.Id} not found.");

        product.UpdateStock(request.StockQuantity);
        await _productRepository.UpdateAsync(product, cancellationToken);

        return ProductResponseDto.FromEntity(product);
    }
}
