namespace Jurupema.Api.Domain.Entities;

public class ProductImage(int productId, string name, string url) : BaseEntity
{
    public string Url { get; private set; } = url;
    public string Name { get; private set; } = name;
    public int ProductId { get; private set; } = productId;
    public Product Product { get; private set; }
}
