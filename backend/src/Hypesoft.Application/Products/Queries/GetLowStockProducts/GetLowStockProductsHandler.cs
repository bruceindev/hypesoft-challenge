using AutoMapper;
using MediatR;
using Hypesoft.Application.Products.DTOs;
using Hypesoft.Domain.Interfaces;

namespace Hypesoft.Application.Products.Queries.GetLowStockProducts;

public class GetLowStockProductsHandler : IRequestHandler<GetLowStockProductsQuery, List<ProductResponseDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetLowStockProductsHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<List<ProductResponseDto>> Handle(
        GetLowStockProductsQuery request,
        CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetLowStockProductsAsync(request.Threshold, cancellationToken);
        return _mapper.Map<List<ProductResponseDto>>(products.ToList());
    }
}
