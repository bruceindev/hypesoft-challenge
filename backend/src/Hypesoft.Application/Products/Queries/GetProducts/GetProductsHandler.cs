using MediatR;
using Hypesoft.Application.Common.Models;
using Hypesoft.Application.Products.DTOs;
using Hypesoft.Domain.Interfaces;

namespace Hypesoft.Application.Products.Queries.GetProducts;

public class GetProductsHandler : IRequestHandler<GetProductsQuery, PagedResponse<ProductResponseDto>>
{
    private readonly IProductRepository _productRepository;

    public GetProductsHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<PagedResponse<ProductResponseDto>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        if (request.PageNumber < 1)
            throw new ArgumentException("Page number must be greater than zero.");

        if (request.PageSize < 1 || request.PageSize > 100)
            throw new ArgumentException("Page size must be between 1 and 100.");

        var products = await _productRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            request.SearchTerm,
            request.CategoryId,
            cancellationToken);

        var totalCount = await _productRepository.CountAsync(
            request.SearchTerm,
            request.CategoryId,
            cancellationToken);

        var productDtos = products
            .Select(ProductResponseDto.FromEntity)
            .ToList();

        return new PagedResponse<ProductResponseDto>(
            productDtos,
            request.PageNumber,
            request.PageSize,
            totalCount);
    }
}
