using Jurupema.Api.Domain.Entities;

namespace Jurupema.Api.Application.Models;

public record ProductListItemResult
{
    public int Id { get; }
    public string Name { get; }
    public string Description { get; }
    public decimal Price { get; }
    public int Stock { get; }
    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; }
    public IReadOnlyList<ProductImageListItemResult> Images { get; }

    private ProductListItemResult(
        int id,
        string name,
        string description,
        decimal price,
        int stock,
        DateTime createdAt,
        DateTime? updatedAt,
        IReadOnlyList<ProductImageListItemResult> images)
    {
        Id = id;
        Name = name;
        Description = description;
        Price = price;
        Stock = stock;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        Images = images;
    }

    /// <summary>
    /// Maps a loaded <see cref="Product"/> and pre-built image rows (e.g. URLs resolved in the application service).
    /// </summary>
    public static ProductListItemResult FromProduct(Product product, IReadOnlyList<ProductImageListItemResult> images) =>
        new(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.Stock,
            product.CreatedAt,
            product.UpdatedAt,
            images);
}
