namespace Jurupema.Api.Application.Exceptions;

public sealed class ProductNotFoundException : Exception
{
    public ProductNotFoundException(Guid productId)
        : base($"Product with id {productId} was not found.")
    {
        ProductId = productId;
    }

    public Guid ProductId { get; }
}
