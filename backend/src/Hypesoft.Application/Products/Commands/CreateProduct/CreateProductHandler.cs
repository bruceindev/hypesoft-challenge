using AutoMapper;
using MediatR;
using Hypesoft.Application.Products.DTOs;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Interfaces;

namespace Hypesoft.Application.Products.Commands.CreateProduct;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductResponseDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CreateProductHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<ProductResponseDto> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var categoryExists = await _categoryRepository.ExistsAsync(request.CategoryId, cancellationToken);
        if (!categoryExists)
            throw new ArgumentException("Category does not exist");

        var product = Product.Create(
            request.Name,
            request.Description,
            request.Price,
            request.StockQuantity,
            request.CategoryId,
            request.ImageUrl);

        await _productRepository.AddAsync(product, cancellationToken);

        return _mapper.Map<ProductResponseDto>(product);
    }
}
