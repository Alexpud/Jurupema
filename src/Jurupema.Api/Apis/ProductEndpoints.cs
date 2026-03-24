using Jurupema.Api.Application.Models;
using Jurupema.Api.Application.Products;
using Jurupema.Api.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Jurupema.Api.Apis;

public static class ProductEndpoints
{
    public static RouteGroupBuilder MapProductEndpoints(this IEndpointRouteBuilder endpointBuilder)
    {
        var group = endpointBuilder.MapGroup("/products");

        group.MapGet("/", async (
                [AsParameters] QueryProductsParameters parameters,
                [FromServices] ProductService productService,
                CancellationToken cancellationToken) =>
            {
                var result = await productService.QueryProductsAsync(parameters, cancellationToken);
                return Results.Ok(result);
            })
            .Produces<OkResult>(StatusCodes.Status200OK)
            .WithFluentValidation<QueryProductsParameters>()
            .WithSummary("Lists products with optional name filter, sorting, paging, and optional image inclusion")
            .WithDescription("Lists products with optional name filter, sorting, paging, and optional image inclusion")
            .WithTags("product");

        group.MapPost("/", async ([FromBody] CreateProductParameter request, [FromServices] ProductService productService) =>
        {
            var result = await productService.CreateProduct(request);
            return Results.Created($"/products/{result.Id}", result);
        })
            .Produces<CreatedResult>(StatusCodes.Status201Created)
            .WithFluentValidation<CreateProductParameter>()
            .WithSummary("Creates a new product")
            .WithDescription("Creates a new product")
            .WithTags("product");

        group.MapPut("/{id:int}", async (
                int id,
                [FromBody] UpdateProductRequest request,
                [FromServices] ProductService productService) =>
            {
                await productService.UpdateProductAsync(new UpdateProductParameter
                {
                    Id = id,
                    Name = request.Name,
                    Description = request.Description,
                    Price = request.Price,
                    Stock = request.Stock
                });
                return Results.NoContent();
            })
            .Produces<NoContentResult>(StatusCodes.Status204NoContent)
            .WithFluentValidation<UpdateProductRequest>()
            .WithSummary("Updates an existing product")
            .WithDescription("Updates an existing product")
            .WithTags("product");

        return group;
    }
}
