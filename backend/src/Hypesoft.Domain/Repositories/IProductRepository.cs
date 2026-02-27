namespace Hypesoft.Domain.Repositories;

using Hypesoft.Domain.Entities;

public interface IProductRepository
{
    Task AddAsync(Product product, CancellationToken cancellationToken = default);

    Task UpdateAsync(Product product, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Product>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        Guid? categoryId = null,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        string? searchTerm = null,
        Guid? categoryId = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Product>> GetLowStockAsync(
        int threshold = 10,
        CancellationToken cancellationToken = default);

    Task<decimal> GetTotalStockValueAsync(CancellationToken cancellationToken = default);
}