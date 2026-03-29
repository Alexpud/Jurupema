namespace Jurupema.Api.Infrastructure.Configurations;

public class ServiceBusTopicsOptions
{
    public string OrderCreated { get; set; }
}

public class ServiceBusSubscriptionsOptions
{
    public string OrderCreated { get; set; }
}

public class ServiceBusConfiguration
{
    public const string Position = "ServiceBus";

    public bool Enabled { get; set; }

    public string ConnectionString { get; set; }

    public ServiceBusTopicsOptions Topics { get; set; } = new();

    public ServiceBusSubscriptionsOptions Subscriptions { get; set; } = new();
}
