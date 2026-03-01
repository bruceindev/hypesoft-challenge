using MediatR;
using Hypesoft.Application.Products.DTOs;

namespace Hypesoft.Application.Products.Queries.GetProductById;

public class GetProductByIdQuery : IRequest<ProductResponseDto>
{
    public Guid Id { get; set; }
}
