namespace Jurupema.Api.Filters;

public static class DomainExceptionFilterExtensions
{
    public static RouteHandlerBuilder WithDomainExceptionHandling(this RouteHandlerBuilder builder) =>
        builder.AddEndpointFilter<DomainExceptionFilter>();

    public static RouteGroupBuilder WithDomainExceptionHandling(this RouteGroupBuilder builder)
    {
        builder.AddEndpointFilter<DomainExceptionFilter>();
        return builder;
    }
}

