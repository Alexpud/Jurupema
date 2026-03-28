using Jurupema.Api.Domain;
using Jurupema.Api.Domain.Entities;

namespace Jurupema.Api.Domain.Repositories;

public interface IProductRepository : IBaseRepository<Product>
{
    Task<IReadOnlyList<Product>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPagedAsync(
        string nameContains,
        ProductSortBy sortBy,
        SortDirection sortDirection,
        int pageIndex,
        int pageSize,
        bool includeImages,
        CancellationToken cancellationToken = default);
}
