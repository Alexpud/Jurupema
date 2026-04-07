using Jurupema.Api.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Jurupema.Api.Filters;

public sealed class DomainExceptionFilter : IEndpointFilter
{
    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        try
        {
            return await next(context);
        }
        catch (DomainException ex)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Domain validation error",
                detail: ex.Message,
                extensions: new Dictionary<string, object>
                {
                    ["code"] = ex.Code
                });
        }
    }
}

