namespace Jurupema.Api.Domain.Entities;

public class Product(string name, string description, decimal price) : BaseEntity
{
    public string Name { get; private set; } = name;
    public string Description { get; set; } = description;
    public decimal Price { get; private set; } = price;
    public int Stock { get; private set; }

    public List<ProductImage> ProductImages { get; set; } = [];

    public void Update(string name, string description, decimal price, int stock)
    {
        Name = name;
        Description = description;
        Price = price;
        Stock = stock;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool TryAddImage(string fileName, string url)
    {
        var alreadyHasImage = ProductImages.Any(p => p.Url.Equals(url));
        if (alreadyHasImage)
            return false;
        
        ProductImages.Add(new ProductImage(Id, fileName, url));
        return true;
    }

    public bool AlreadyHasImage(ProductImage productImage)
    {
        return ProductImages.Any(p => p.Url.Equals(productImage));
    }
}
