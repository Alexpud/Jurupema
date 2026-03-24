using Jurupema.Api.Application.Products;
using Microsoft.AspNetCore.Mvc;

namespace Jurupema.Api.Apis;

public static class ProductImageEndpoints
{
    public static RouteGroupBuilder MapProductImageEndpoints(this IEndpointRouteBuilder endpointBuilder)
    {
        var group = endpointBuilder.MapGroup("/{productId}/image");


        group.MapDelete("/{productImageId:int}", async (
                [FromRoute] int productId,
                [FromRoute] int productImageId,
                [FromServices] ProductImageService productImageService,
                CancellationToken cancellationToken) =>
            {
                await productImageService.DeleteProductImageAsync(productId, productImageId, cancellationToken);
                return Results.NoContent();
            })
            .Produces<NoContentResult>(StatusCodes.Status204NoContent)
            .WithSummary("Deletes a product image")
            .WithDescription(
                "Removes the image row for this product and deletes the blob from storage. Returns 204 when successful.")
            .WithTags("product");

        group.MapPost("/", async ([FromRoute] int productId, [FromServices] ProductImageService productImageService, IFormFile file) =>
        {
            await productImageService.UploadProductImageAsync(productId, file.FileName, file.OpenReadStream());
            return Results.Ok();
        })
            .Produces<OkResult>(StatusCodes.Status200OK)
            .DisableAntiforgery()
            .WithSummary("Uploads a product image")
            .WithDescription(
                "Accepts multipart/form-data with the image file. The uploaded name must not duplicate an existing image on the same product. " +
                "On success the file is stored in blob storage and persisted as a product image record.")
            .WithTags("product");

        return group;
    }
}
