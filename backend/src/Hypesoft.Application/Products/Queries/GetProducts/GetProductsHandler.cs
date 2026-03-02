using AutoMapper;
using MediatR;
using Hypesoft.Application.Products.DTOs;
using Hypesoft.Domain.Interfaces;

namespace Hypesoft.Application.Products.Queries.GetProducts;

public class GetProductsHandler : IRequestHandler<GetProductsQuery, PagedResult<ProductResponseDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetProductsHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<ProductResponseDto>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        var page = await _productRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            request.SearchTerm,
            request.CategoryId,
            cancellationToken);

        return new PagedResult<ProductResponseDto>
        {
            Items = _mapper.Map<List<ProductResponseDto>>(page.Items),
            TotalCount = page.TotalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
