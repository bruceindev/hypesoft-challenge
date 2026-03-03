using MediatR;
using Hypesoft.Domain.Interfaces;

namespace Hypesoft.Application.Categories.Commands.DeleteCategory;

public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, Unit>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICacheService _cacheService;

    public DeleteCategoryHandler(ICategoryRepository categoryRepository, ICacheService cacheService)
    {
        _categoryRepository = categoryRepository;
        _cacheService = cacheService;
    }

    public async Task<Unit> Handle(
        DeleteCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category == null)
            throw new KeyNotFoundException($"Category with ID {request.Id} not found");

        await _categoryRepository.DeleteAsync(request.Id, cancellationToken);
        await _cacheService.RemoveAsync("categories:list", cancellationToken);
        await _cacheService.RemoveAsync("dashboard:stats", cancellationToken);

        return Unit.Value;
    }
}
