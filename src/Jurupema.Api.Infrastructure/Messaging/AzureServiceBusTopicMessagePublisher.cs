using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Jurupema.Api.Application.Messaging;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace Jurupema.Api.Infrastructure.Messaging;

public sealed class AzureServiceBusTopicMessagePublisher(ServiceBusClient client, ILogger<AzureServiceBusTopicMessagePublisher> logger) : ITopicMessagePublisher, IAsyncDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly ServiceBusClient _client = client;
    private readonly ConcurrentDictionary<string, ServiceBusSender> _senders = new();
    private readonly ILogger<AzureServiceBusTopicMessagePublisher> _logger = logger;

    public async Task PublishAsync<T>(
        string topicName,
        string messageName,
        T message,
        CancellationToken cancellationToken = default)
    {
        var sender = _senders.GetOrAdd(topicName, _client.CreateSender);
        var json = JsonSerializer.Serialize(message, JsonOptions);
        var serviceBusMessage = new ServiceBusMessage(BinaryData.FromString(json))
        {
            Subject = messageName,
            ContentType = "application/json"
        };

        InjectTraceContext(serviceBusMessage);

        await sender.SendMessageAsync(serviceBusMessage, cancellationToken);

        _logger.LogDebug(
            "Message={Message}; TopicName={TopicName}",
            $"Published message",
            topicName);
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var sender in _senders.Values)
            await sender.DisposeAsync();

        _senders.Clear();
    }

    private static void InjectTraceContext(ServiceBusMessage message)
    {
        var activity = Activity.Current;
        if (activity is null)
            return;

        Propagators.DefaultTextMapPropagator.Inject(
            new PropagationContext(activity.Context, Baggage.Current),
            message.ApplicationProperties,
            static (props, key, value) => { props[key] = value; });
    }
}
