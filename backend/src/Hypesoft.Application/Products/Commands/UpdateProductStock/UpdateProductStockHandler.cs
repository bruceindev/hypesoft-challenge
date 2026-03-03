using AutoMapper;
using MediatR;
using Hypesoft.Application.Products.DTOs;
using Hypesoft.Domain.Interfaces;

namespace Hypesoft.Application.Products.Commands.UpdateProductStock;

public class UpdateProductStockHandler : IRequestHandler<UpdateProductStockCommand, ProductResponseDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public UpdateProductStockHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
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

        return _mapper.Map<ProductResponseDto>(product);
    }
}
