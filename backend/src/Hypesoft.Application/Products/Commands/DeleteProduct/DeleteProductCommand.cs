using MediatR;

namespace Hypesoft.Application.Products.Commands.DeleteProduct;

public class DeleteProductCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}
