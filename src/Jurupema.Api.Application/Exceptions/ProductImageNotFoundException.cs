namespace Jurupema.Api.Application.Exceptions;

public sealed class ProductImageNotFoundException : Exception
{
    public ProductImageNotFoundException(Guid productId, Guid productImageId)
        : base($"Product image with id {productImageId} was not found for product {productId}.")
    {
        ProductId = productId;
        ProductImageId = productImageId;
    }

    public Guid ProductId { get; }
    public Guid ProductImageId { get; }
}
