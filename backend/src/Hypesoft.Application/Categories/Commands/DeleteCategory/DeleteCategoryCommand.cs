using MediatR;

namespace Hypesoft.Application.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommand : IRequest<Unit>
{
    public string Id { get; set; } = string.Empty;
}
