using MediatR;
using Hypesoft.Application.Products.DTOs;
using Hypesoft.Domain.Interfaces;

namespace Hypesoft.Application.Products.Queries.GetLowStockProducts;

public class GetLowStockProductsHandler : IRequestHandler<GetLowStockProductsQuery, IReadOnlyList<ProductResponseDto>>
{
    private readonly IProductRepository _productRepository;

    public GetLowStockProductsHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IReadOnlyList<ProductResponseDto>> Handle(
        GetLowStockProductsQuery request,
        CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetLowStockAsync(request.Threshold, cancellationToken);

        return products
            .Select(ProductResponseDto.FromEntity)
            .ToList();
    }
}
