using FluentValidation;

namespace Jurupema.Api.Filters;

public class FluentValidationFilter<TParameter> : IEndpointFilter
{
    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var validator = context.HttpContext.RequestServices.GetService<IValidator<TParameter>>();
        if (validator is null)
            return await next(context);

        var parameter = context.Arguments.OfType<TParameter>().FirstOrDefault()
            ?? throw new InvalidOperationException(
                $"No argument of type {typeof(TParameter).Name} was found in the endpoint. " +
                "Ensure the validated request type is used as a handler parameter.");

        var validationResult = await validator.ValidateAsync(parameter);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            return Results.ValidationProblem(errors);
        }

        return await next(context);
    }
}
