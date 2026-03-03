using MediatR;
using Hypesoft.Application.Products.DTOs;

namespace Hypesoft.Application.Products.Queries.GetProductById;

public class GetProductByIdQuery : IRequest<ProductResponseDto>
{
    public string Id { get; set; } = string.Empty;
}
