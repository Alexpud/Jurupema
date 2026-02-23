using Jurupema.Api.Application.Storage;

namespace Jurupema.Api.Apis;

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
