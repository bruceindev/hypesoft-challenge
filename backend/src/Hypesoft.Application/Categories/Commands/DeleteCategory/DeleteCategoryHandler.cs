using MediatR;
using Hypesoft.Domain.Interfaces;

namespace Hypesoft.Application.Categories.Commands.DeleteCategory;

public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, Unit>
{
    private readonly ICategoryRepository _categoryRepository;

    public DeleteCategoryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Unit> Handle(
        DeleteCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category == null)
            throw new KeyNotFoundException($"Category with ID {request.Id} not found.");

        var hasProducts = await _categoryRepository.HasProductsAsync(request.Id, cancellationToken);

        if (hasProducts)
            throw new InvalidOperationException("Cannot delete category with associated products.");

        await _categoryRepository.DeleteAsync(request.Id, cancellationToken);

        return Unit.Value;
    }
}
