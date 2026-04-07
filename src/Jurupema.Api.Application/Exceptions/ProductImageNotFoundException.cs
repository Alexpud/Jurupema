namespace Jurupema.Api.Application.Exceptions;

public sealed class ProductImageNotFoundException : DomainException
{
    public ProductImageNotFoundException(Guid productId, Guid productImageId)
        : base("product_image_not_found", $"Product image with id {productImageId} was not found for product {productId}.")
    {
        ProductId = productId;
        ProductImageId = productImageId;
    }

    public Guid ProductId { get; }
    public Guid ProductImageId { get; }
}
