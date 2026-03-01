using MediatR;
using Hypesoft.Domain.Interfaces;

namespace Hypesoft.Application.Products.Commands.DeleteProduct;

public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, Unit>
{
    private readonly IProductRepository _productRepository;

    public DeleteProductHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Unit> Handle(
        DeleteProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

        if (product == null)
            throw new KeyNotFoundException($"Product with ID {request.Id} not found.");

        await _productRepository.DeleteAsync(request.Id, cancellationToken);

        return Unit.Value;
    }
}
