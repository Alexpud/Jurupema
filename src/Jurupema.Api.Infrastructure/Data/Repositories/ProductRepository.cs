using Jurupema.Api.Domain;
using Jurupema.Api.Domain.Entities;
using Jurupema.Api.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Jurupema.Api.Infrastructure.Data.Repositories;

public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(JurupemaDbContext context) : base(context)
    {
    }

    public async Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPagedAsync(
        string nameContains,
        ProductSortBy sortBy,
        SortDirection sortDirection,
        int pageIndex,
        int pageSize,
        bool includeImages,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(pageIndex);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(pageSize, 0);

        IQueryable<Product> query = DbSet;

        if (includeImages)
            query = query.Include(p => p.ProductImages);

        if (!string.IsNullOrWhiteSpace(nameContains))
        {
            var term = nameContains.Trim();
            query = query.Where(p => p.Name.Contains(term));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        query = ApplySort(query, sortBy, sortDirection);

        var items = await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    private static IQueryable<Product> ApplySort(
        IQueryable<Product> query,
        ProductSortBy sortBy,
        SortDirection sortDirection)
    {
        return sortBy switch
        {
            ProductSortBy.Name when sortDirection == SortDirection.Ascending => query.OrderBy(p => p.Name),
            ProductSortBy.Name => query.OrderByDescending(p => p.Name),
            ProductSortBy.Price when sortDirection == SortDirection.Ascending => query.OrderBy(p => p.Price),
            ProductSortBy.Price => query.OrderByDescending(p => p.Price),
            ProductSortBy.CreatedAt when sortDirection == SortDirection.Ascending => query.OrderBy(p => p.CreatedAt),
            ProductSortBy.CreatedAt => query.OrderByDescending(p => p.CreatedAt),
            ProductSortBy.Stock when sortDirection == SortDirection.Ascending => query.OrderBy(p => p.Stock),
            ProductSortBy.Stock => query.OrderByDescending(p => p.Stock),
            _ => query.OrderBy(p => p.Id)
        };
    }
}

