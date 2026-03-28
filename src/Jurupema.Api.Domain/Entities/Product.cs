using Jurupema.Api.Domain;

namespace Jurupema.Api.Domain.Entities;

public class Product(string name, string description, decimal price) : BaseEntity, IUpdatable
{
    public const int MaxNameLength = 200;
    public const int MaxDescriptionLength = 1000;

    public List<ProductOrder> ProductOrders { get; private set; } = [];
    public string Name { get; private set; } = Guards.ThrowIfNullWhiteSpaceOrTooLong(name, nameof(name), MaxNameLength);
    public string Description { get; private set; } = Guards.ThrowIfNullWhiteSpaceOrTooLong(description, nameof(description), MaxDescriptionLength);
    public decimal Price { get; private set; } = Guards.ThrowIfNegative(price, nameof(price));
    public int Stock { get; private set; }

    public List<ProductImage> ProductImages { get; private set; } = [];
    public DateTime? UpdatedAt { get; private set; }

    public void Update(string name, string description, decimal price, int stock)
    {
        Name = Guards.ThrowIfNullWhiteSpaceOrTooLong(name, nameof(name), MaxNameLength);
        Description = Guards.ThrowIfNullWhiteSpaceOrTooLong(description, nameof(description), MaxDescriptionLength);
        Price = Guards.ThrowIfNegative(price, nameof(price));
        Stock = Guards.ThrowIfNegative(stock, nameof(stock));
        UpdatedAt = DateTime.UtcNow;
    }

    public bool TryAddImage(string fileName)
    {
        var normalized = ProductImage.NormalizeName(fileName, nameof(fileName));
        if (ProductImages.Any(p => p.Name.Equals(normalized, StringComparison.Ordinal)))
            return false;

        ProductImages.Add(new ProductImage(Id, normalized));
        return true;
    }
}
