using AutoMapper;
using MediatR;
using Hypesoft.Application.Categories.DTOs;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Interfaces;

namespace Hypesoft.Application.Categories.Commands.CreateCategory;

public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, CategoryResponseDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CreateCategoryHandler(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<CategoryResponseDto> Handle(
        CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = Category.Create(request.Name, request.Description);
        await _categoryRepository.AddAsync(category, cancellationToken);

        return _mapper.Map<CategoryResponseDto>(category);
    }
}
