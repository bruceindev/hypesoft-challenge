using AutoMapper;
using MediatR;
using Hypesoft.Application.Products.DTOs;
using Hypesoft.Domain.Interfaces;

namespace Hypesoft.Application.Products.Commands.UpdateProductStock;

public class UpdateProductStockHandler : IRequestHandler<UpdateProductStockCommand, ProductResponseDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;

    public UpdateProductStockHandler(IProductRepository productRepository, ICacheService cacheService, IMapper mapper)
    {
        _productRepository = productRepository;
        _cacheService = cacheService;
        _mapper = mapper;
    }

    public async Task<ProductResponseDto> Handle(
        UpdateProductStockCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {request.Id} not found");

        product.UpdateStock(request.StockQuantity);

        await _productRepository.UpdateAsync(product, cancellationToken);
        await _cacheService.RemoveAsync("dashboard:stats", cancellationToken);
        await _cacheService.RemoveAsync("categories:list", cancellationToken);

        return _mapper.Map<ProductResponseDto>(product);
    }
}
