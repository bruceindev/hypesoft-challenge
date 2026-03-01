using MediatR;
using Hypesoft.Application.Products.DTOs;
using Hypesoft.Domain.Interfaces;

namespace Hypesoft.Application.Products.Commands.UpdateProduct;

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, ProductResponseDto>
{
    private readonly IProductRepository _productRepository;

    public UpdateProductHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductResponseDto> Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Product name is required.");

        if (request.Name.Length > 150)
            throw new ArgumentException("Product name cannot exceed 150 characters.");

        if (request.Description?.Length > 500)
            throw new ArgumentException("Product description cannot exceed 500 characters.");

        if (request.Price <= 0)
            throw new ArgumentException("Price must be greater than zero.");

        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

        if (product == null)
            throw new KeyNotFoundException($"Product with ID {request.Id} not found.");

        var nameExists = await _productRepository.ExistsByNameExcludingIdAsync(
            request.Name,
            request.Id,
            cancellationToken);

        if (nameExists)
            throw new InvalidOperationException($"A product with the name '{request.Name}' already exists.");

        product.UpdateDetails(request.Name, request.Description ?? string.Empty, request.Price);
        await _productRepository.UpdateAsync(product, cancellationToken);

        return ProductResponseDto.FromEntity(product);
    }
}
