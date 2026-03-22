namespace Jurupema.Api.Application.Exceptions;

public sealed class ProductNotFoundException : Exception
{
    public ProductNotFoundException(int productId)
        : base($"Product with id {productId} was not found.")
    {
        ProductId = productId;
    }

    public int ProductId { get; }
}
