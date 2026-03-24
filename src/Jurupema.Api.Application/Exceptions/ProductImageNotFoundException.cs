namespace Jurupema.Api.Application.Exceptions;

public sealed class ProductImageNotFoundException : Exception
{
    public ProductImageNotFoundException(int productId, int productImageId)
        : base($"Product image with id {productImageId} was not found for product {productId}.")
    {
        ProductId = productId;
        ProductImageId = productImageId;
    }

    public int ProductId { get; }
    public int ProductImageId { get; }
}
