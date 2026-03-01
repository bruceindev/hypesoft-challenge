using MediatR;
using Hypesoft.Domain.Interfaces;

namespace Hypesoft.Application.Products.Queries.GetTotalStockValue;

public class GetTotalStockValueHandler : IRequestHandler<GetTotalStockValueQuery, decimal>
{
    private readonly IProductRepository _productRepository;

    public GetTotalStockValueHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<decimal> Handle(
        GetTotalStockValueQuery request,
        CancellationToken cancellationToken)
    {
        return await _productRepository.GetTotalStockValueAsync(cancellationToken);
    }
}
