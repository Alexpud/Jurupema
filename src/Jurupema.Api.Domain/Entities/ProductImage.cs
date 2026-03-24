namespace Jurupema.Api.Domain.Entities;

public class ProductImage(int productId, string name) : BaseEntity
{
    public string Name { get; private set; } = name;
    public int ProductId { get; private set; } = productId;
    public Product Product { get; private set; }
}
