using AutoMapper;
using MediatR;
using Hypesoft.Application.Categories.DTOs;
using Hypesoft.Domain.Interfaces;

namespace Hypesoft.Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, CategoryResponseDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public UpdateCategoryHandler(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<CategoryResponseDto> Handle(
        UpdateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category == null)
            throw new KeyNotFoundException($"Category with ID {request.Id} not found");

        category.Update(request.Name, request.Description);
        await _categoryRepository.UpdateAsync(category, cancellationToken);

        return _mapper.Map<CategoryResponseDto>(category);
    }
}
