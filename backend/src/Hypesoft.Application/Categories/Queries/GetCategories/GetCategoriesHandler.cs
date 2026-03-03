using AutoMapper;
using MediatR;
using Hypesoft.Application.Categories.DTOs;
using Hypesoft.Domain.Interfaces;

namespace Hypesoft.Application.Categories.Queries.GetCategories;

public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, IReadOnlyList<CategoryResponseDto>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public GetCategoriesHandler(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<CategoryResponseDto>> Handle(
        GetCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<List<CategoryResponseDto>>(categories.ToList());
    }
}
