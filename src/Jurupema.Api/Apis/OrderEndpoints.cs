using Jurupema.Api.Application.Models;
using Jurupema.Api.Application.Services.Orders;
using Jurupema.Api.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Jurupema.Api.Apis;

public static class OrderEndpoints
{
    public static RouteGroupBuilder MapOrderEndpoints(this IEndpointRouteBuilder endpointBuilder)
    {
        var group = endpointBuilder.MapGroup("/orders");

        group.MapPost("/", async (
                [FromBody] CreateOrderParameter request,
                [FromServices] OrderService orderService,
                CancellationToken cancellationToken) =>
            {
                var result = await orderService.CreateOrderAsync(request, cancellationToken);
                return Results.Created($"/orders/{result.OrderId}", result);
            })
            .Produces<CreateOrderResult>(StatusCodes.Status201Created)
            .WithFluentValidation<CreateOrderParameter>()
            .WithSummary("Creates a new order with one or more product lines")
            .WithDescription("Each line references a catalog product and quantity; line totals use the current product price at order time.")
            .WithTags("order");

        return group;
    }
}
