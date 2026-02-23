using Jurupema.Api.Application.Products;
using Microsoft.AspNetCore.Mvc;

namespace Jurupema.Api.Apis;

public static class ProductImageEndpoints
{
    public static RouteGroupBuilder MapProductImageEndpoints(this IEndpointRouteBuilder endpointBuilder)
    {
        var group = endpointBuilder.MapGroup("/{productId}/image");
        group.MapPost("/", async ([FromRoute] int productId, [FromServices] ProductService productService, IFormFile file) =>
        {
            await productService.UploadProductImageAsync(productId, file.FileName, file.OpenReadStream());
            return Results.Ok();
        }).DisableAntiforgery();

        return group;
    }
}