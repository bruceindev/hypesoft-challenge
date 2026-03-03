using AutoMapper;
using MediatR;
using Hypesoft.Application.Categories.DTOs;
using Hypesoft.Domain.Interfaces;

namespace Hypesoft.Application.Categories.Queries.GetCategories;

public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, IReadOnlyList<CategoryResponseDto>>
{
    private const string CategoriesCacheKey = "categories:list";

    private readonly ICategoryRepository _categoryRepository;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;

    public GetCategoriesHandler(ICategoryRepository categoryRepository, ICacheService cacheService, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _cacheService = cacheService;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<CategoryResponseDto>> Handle(
        GetCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var cachedCategories = await _cacheService.GetAsync<List<CategoryResponseDto>>(CategoriesCacheKey, cancellationToken);
        if (cachedCategories != null)
        {
            return cachedCategories;
        }

        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        var mappedCategories = _mapper.Map<List<CategoryResponseDto>>(categories.ToList());

        await _cacheService.SetAsync(CategoriesCacheKey, mappedCategories, TimeSpan.FromMinutes(5), cancellationToken);

        return mappedCategories;
    }
}
