using Microsoft.AspNetCore.Builder;

namespace Jurupema.Api.Filters;

public static class FluentValidationFilterExtensions
{
    /// <summary>
    /// Adds FluentValidation for the given request type. If an <see cref="FluentValidation.IValidator{T}"/> is registered, the endpoint argument of type <typeparamref name="T"/> is validated before the handler runs.
    /// </summary>
    public static RouteHandlerBuilder WithFluentValidation<T>(this RouteHandlerBuilder builder) =>
        builder.AddEndpointFilter<FluentValidationFilter<T>>();
}
