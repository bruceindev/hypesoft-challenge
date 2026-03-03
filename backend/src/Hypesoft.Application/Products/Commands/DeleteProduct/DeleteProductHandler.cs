using MediatR;
using Hypesoft.Domain.Interfaces;

namespace Hypesoft.Application.Products.Commands.DeleteProduct;

public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, Unit>
{
    private readonly IProductRepository _productRepository;
    private readonly ICacheService _cacheService;

    public DeleteProductHandler(IProductRepository productRepository, ICacheService cacheService)
    {
        _productRepository = productRepository;
        _cacheService = cacheService;
    }

    public async Task<Unit> Handle(
        DeleteProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {request.Id} not found");

        await _productRepository.DeleteAsync(request.Id, cancellationToken);
        await _cacheService.RemoveAsync("dashboard:stats", cancellationToken);
        await _cacheService.RemoveAsync("categories:list", cancellationToken);

        return Unit.Value;
    }
}
