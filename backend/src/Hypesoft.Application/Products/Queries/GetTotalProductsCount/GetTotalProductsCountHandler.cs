using MediatR;
using Hypesoft.Domain.Interfaces;

namespace Hypesoft.Application.Products.Queries.GetTotalProductsCount;

public class GetTotalProductsCountHandler : IRequestHandler<GetTotalProductsCountQuery, int>
{
    private readonly IProductRepository _productRepository;

    public GetTotalProductsCountHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<int> Handle(
        GetTotalProductsCountQuery request,
        CancellationToken cancellationToken)
    {
        return await _productRepository.CountAsync(null, null, cancellationToken);
    }
}
