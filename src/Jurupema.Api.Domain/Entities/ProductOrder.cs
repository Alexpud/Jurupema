using Jurupema.Api.Domain;

namespace Jurupema.Api.Domain.Entities;

public class ProductOrder(Guid orderId, Guid productId, decimal price) : BaseEntity
{
    public Guid OrderId { get; private set; } = Guards.ThrowIfDefault(orderId, nameof(orderId));
    public Order Order { get; internal set; }
    public Guid ProductId { get; private set; } = Guards.ThrowIfDefault(productId, nameof(productId));
    public Product Product { get; internal set; }
    public decimal Price { get; private set; } = Guards.ThrowIfNegative(price, nameof(price));
}
