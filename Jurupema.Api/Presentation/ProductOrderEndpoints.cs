using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Jurupema.Api.Presentation;

public static class ProductOrderEndpoints
{
    public static RouteGroupBuilder MapProductOrderEndpoints(this IEndpointRouteBuilder endpointBuilder)
    {
        var group = endpointBuilder.MapGroup("/product-orders");
        group
            .MapGet("/", () => {
                return new {
                    Id = 1,
                    Name = "Product 1",
                    Description = "Description 1"
                };
            });
        return group;
    }
}