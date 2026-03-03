using MediatR;
using Hypesoft.Application.Categories.DTOs;

namespace Hypesoft.Application.Categories.Commands.CreateCategory;

public class CreateCategoryCommand : IRequest<CategoryResponseDto>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
