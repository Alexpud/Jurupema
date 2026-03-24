namespace Jurupema.Api.Domain.Entities;

public class ProductImage(Guid productId, string name) : BaseEntity
{
    public string Name { get; private set; } = name;
    public Guid ProductId { get; private set; } = productId;
    public Product Product { get; private set; }
}
