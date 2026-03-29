using Jurupema.Api.Application.Messaging;
using Jurupema.Api.Infrastructure.Configurations;
using Microsoft.Extensions.Options;

namespace Jurupema.Api.Infrastructure.Messaging;

public sealed class OrderServiceBusTopics : IOrderServiceBusTopics
{
    public OrderServiceBusTopics(IOptions<ServiceBusConfiguration> options)
    {
        var topic = options.Value.Topics?.OrderCreated;
        if (string.IsNullOrWhiteSpace(topic))
            throw new InvalidOperationException(
                "ServiceBus:Topics:OrderCreated must be configured with a non-empty topic name.");

        OrderCreated = topic;
    }

    public string OrderCreated { get; }
}
