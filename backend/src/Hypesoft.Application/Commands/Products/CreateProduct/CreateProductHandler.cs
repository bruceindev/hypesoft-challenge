using MediatR;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;

namespace Hypesoft.Application.Commands.Products.CreateProduct;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, Guid>
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

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var categoryExists = await _categoryRepository.ExistsAsync(request.CategoryId, cancellationToken);

        if (!categoryExists)
            throw new ArgumentException("Category does not exist.");

        var product = new Product(
            request.Name,
            request.Description,
            request.Price,
            request.CategoryId,
            request.StockQuantity);

        await _productRepository.AddAsync(product, cancellationToken);

        return product.Id;
    }
}