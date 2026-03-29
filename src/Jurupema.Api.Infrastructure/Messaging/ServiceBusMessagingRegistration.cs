using Azure.Messaging.ServiceBus;
using Jurupema.Api.Application.Messaging;
using Jurupema.Api.Infrastructure.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Jurupema.Api.Infrastructure.Messaging;

internal static class ServiceBusMessagingRegistration
{
    public static void RegisterServiceBus(this WebApplicationBuilder builder)
    {
        var serviceBusSection = builder.Configuration.GetSection(ServiceBusConfiguration.Position);
        var serviceBusBootstrap = serviceBusSection.Get<ServiceBusConfiguration>() ?? new ServiceBusConfiguration();
        if (IsEnabled(builder.Configuration, serviceBusBootstrap))
        {
            var connectionString = ResolveConnectionString(
                builder.Configuration,
                serviceBusBootstrap);
            builder.Services.AddSingleton(_ => new ServiceBusClient(connectionString));
            builder.Services.AddSingleton<AzureServiceBusTopicMessagePublisher>();
            builder.Services.AddSingleton<ITopicMessagePublisher>(sp =>
                sp.GetRequiredService<AzureServiceBusTopicMessagePublisher>());
            builder.Services.AddHostedService<OrderCreatedTopicConsumer>();
        }
        else
        {
            builder.Services.AddSingleton<ITopicMessagePublisher, NoOpTopicMessagePublisher>();
        }
    }

    public static bool IsEnabled(IConfiguration configuration, ServiceBusConfiguration options)
    {
        return options.Enabled && !string.IsNullOrWhiteSpace(ResolveConnectionString(configuration, options));
    }

    public static string ResolveConnectionString(IConfiguration configuration, ServiceBusConfiguration options)
    {
        var connectionString = options.ConnectionString;
        if (string.IsNullOrWhiteSpace(connectionString))
            connectionString = configuration.GetConnectionString("messaging");

        return string.IsNullOrWhiteSpace(connectionString) ? null : connectionString;
    }
}
