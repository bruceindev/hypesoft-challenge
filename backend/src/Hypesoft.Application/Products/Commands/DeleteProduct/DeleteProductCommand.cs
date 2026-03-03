using MediatR;

namespace Hypesoft.Application.Products.Commands.DeleteProduct;

public class DeleteProductCommand : IRequest<Unit>
{
    public string Id { get; set; } = string.Empty;
}
