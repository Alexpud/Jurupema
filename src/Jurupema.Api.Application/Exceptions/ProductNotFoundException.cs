namespace Jurupema.Api.Application.Exceptions;

public sealed class ProductNotFoundException : DomainException
{
    public ProductNotFoundException(Guid productId)
        : base("product_not_found", $"Product with id {productId} was not found.")
    {
        ProductId = productId;
    }

    public Guid ProductId { get; }
}
