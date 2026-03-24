using Jurupema.Api.Domain;

namespace Jurupema.Api.Application.Models;

public sealed record QueryProductsParameters(
    ProductSortBy SortBy = ProductSortBy.Name,
    SortDirection SortDirection = SortDirection.Ascending,
    string NameFilter = null,
    int PageIndex = 0,
    int PageSize = 20,
    bool IncludeImages = false);
