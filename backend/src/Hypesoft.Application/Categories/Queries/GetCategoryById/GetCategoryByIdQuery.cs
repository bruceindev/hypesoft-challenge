using MediatR;
using Hypesoft.Application.Categories.DTOs;

namespace Hypesoft.Application.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQuery : IRequest<CategoryResponseDto>
{
    public string Id { get; set; } = string.Empty;
}
