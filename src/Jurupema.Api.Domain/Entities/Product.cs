namespace Jurupema.Api.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public List<ProductImage> ProductImages { get; set; } = [];

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

public class ProductImage(int productId, string name, string url)
{
    public int Id { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.Now;
    public string Url { get; private set; } = url;
    public string Name { get; private set; } = name;
    public int ProductId { get; private set; } = productId;
    public Product Product { get; private set; }
}
