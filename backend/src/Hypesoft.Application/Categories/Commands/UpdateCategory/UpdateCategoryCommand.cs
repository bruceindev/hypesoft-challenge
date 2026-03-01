using MediatR;
using Hypesoft.Application.Categories.DTOs;

namespace Hypesoft.Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommand : IRequest<CategoryResponseDto>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
