using Azure.Identity;
using Azure.Storage.Blobs;
using Jurupema.Api.Infrastructure.Files;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Jurupema.Api.Presentation;

public static class ProductOrderEndpoints
{
    public static RouteGroupBuilder MapProductOrderEndpoints(this IEndpointRouteBuilder endpointBuilder)
    {
        var group = endpointBuilder.MapGroup("/product-orders");
        group
            .MapGet("/", () =>
            {
                return new
                {
                    Id = 1,
                    Name = "Product 1",
                    Description = "Description 1"
                };
            });
        return group;
    }
}

public static class ProductImageEndpoints
{
    public static RouteGroupBuilder MapProductImageEndpoints(this IEndpointRouteBuilder endpointBuilder)
    {
        var group = endpointBuilder.MapGroup("/product-images");
        group.MapPost("/", async ([FromServices] IStorageClient storageClient, IFormFile file) =>
        {
            await storageClient.UploadFile(file.OpenReadStream(), file.FileName);
            return Results.Ok();
        }).DisableAntiforgery();
        return group;
    }
}