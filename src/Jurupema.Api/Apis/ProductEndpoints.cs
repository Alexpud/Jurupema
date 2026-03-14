using Jurupema.Api.Application.Models;
using Jurupema.Api.Application.Products;
using Jurupema.Api.Application.Storage;
using Jurupema.Api.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Jurupema.Api.Apis;

public static class ProductEndpoints
{
    public static RouteGroupBuilder MapProductEndpoints(this IEndpointRouteBuilder endpointBuilder)
    {
        var group = endpointBuilder.MapGroup("/products");
        group.MapPost("/", async ([FromBody] CreateProductParameter request, [FromServices] ProductService productService) =>
        {
            return Results.Ok();
        })
            .WithFluentValidation<CreateProductParameter>()
            .WithDescription("Creates a new product")
        .WithTags("product");
        return group;
    }
}
