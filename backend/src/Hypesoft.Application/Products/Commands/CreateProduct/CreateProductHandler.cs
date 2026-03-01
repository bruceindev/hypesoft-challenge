using MediatR;
using Hypesoft.Application.Products.DTOs;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Interfaces;

namespace Hypesoft.Application.Products.Commands.CreateProduct;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductResponseDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public CreateProductHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<ProductResponseDto> Handle(
        CreateProductCommand request,
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

        if (request.StockQuantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative.");

        if (request.CategoryId == Guid.Empty)
            throw new ArgumentException("Category is required.");

        var categoryExists = await _categoryRepository.ExistsAsync(
            request.CategoryId,
            cancellationToken);

        if (!categoryExists)
            throw new ArgumentException("Category does not exist.");

        var nameExists = await _productRepository.ExistsByNameAsync(
            request.Name,
            cancellationToken);

        if (nameExists)
            throw new InvalidOperationException($"A product with the name '{request.Name}' already exists.");

        var product = new Product(
            request.Name,
            request.Description ?? string.Empty,
            request.Price,
            request.CategoryId,
            request.StockQuantity);

        await _productRepository.AddAsync(product, cancellationToken);

        return ProductResponseDto.FromEntity(product);
    }
}
