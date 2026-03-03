using AutoMapper;
using MediatR;
using Hypesoft.Application.Categories.DTOs;
using Hypesoft.Domain.Interfaces;

namespace Hypesoft.Application.Categories.Queries.GetCategoryById;

public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, CategoryResponseDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public GetCategoryByIdHandler(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<CategoryResponseDto> Handle(
        GetCategoryByIdQuery request,
        CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category == null)
            throw new KeyNotFoundException($"Category with ID {request.Id} not found");

        return _mapper.Map<CategoryResponseDto>(category);
    }
}
