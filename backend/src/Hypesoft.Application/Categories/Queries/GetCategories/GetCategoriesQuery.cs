using MediatR;
using Hypesoft.Application.Categories.DTOs;

namespace Hypesoft.Application.Categories.Queries.GetCategories;

public class GetCategoriesQuery : IRequest<IReadOnlyList<CategoryResponseDto>>
{
}
