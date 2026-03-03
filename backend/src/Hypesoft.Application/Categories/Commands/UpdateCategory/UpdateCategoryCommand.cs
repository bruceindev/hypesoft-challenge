using MediatR;
using Hypesoft.Application.Categories.DTOs;

namespace Hypesoft.Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommand : IRequest<CategoryResponseDto>
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
