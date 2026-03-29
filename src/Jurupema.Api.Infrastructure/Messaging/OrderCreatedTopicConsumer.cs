using System.Diagnostics;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Jurupema.Api.Application.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Jurupema.Api.Infrastructure.Configurations;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace Jurupema.Api.Infrastructure.Messaging;

/// <summary>
/// Consumes OrderCreated messages from the Service Bus subscription. Completes messages when the order
/// is updated or missing (no retry / DLQ). Abandons on unexpected handler failures so the message can be retried.
/// </summary>
public sealed class OrderCreatedTopicConsumer(
    ServiceBusClient client,
    IOrderServiceBusTopics topics,
    IOptions<ServiceBusConfiguration> serviceBusOptions,
    IServiceScopeFactory scopeFactory,
    ILogger<OrderCreatedTopicConsumer> logger) : BackgroundService, IAsyncDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly ServiceBusClient _client = client;
    private readonly IOrderServiceBusTopics _topics = topics;
    private readonly IOptions<ServiceBusConfiguration> _serviceBusOptions = serviceBusOptions;
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger<OrderCreatedTopicConsumer> _logger = logger;
    private ServiceBusProcessor _processor;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var subscription = _serviceBusOptions.Value.Subscriptions?.OrderCreated;
        if (string.IsNullOrWhiteSpace(subscription))
        {
            throw new InvalidOperationException(
                "ServiceBus:Subscriptions:OrderCreated must be configured with a non-empty subscription name.");
        }

        var options = new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = false,
            MaxConcurrentCalls = 1
        };

        _processor = _client.CreateProcessor(_topics.OrderCreated, subscription, options);
        _processor.ProcessMessageAsync += OnMessageAsync;
        _processor.ProcessErrorAsync += OnErrorAsync;

        await _processor.StartProcessingAsync(stoppingToken);

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
        finally
        {
            await _processor.StopProcessingAsync(CancellationToken.None);
        }
    }

    private Task OnErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError(
            args.Exception,
            "Message={Message}; OrderId={OrderId}",
            $"Service Bus processor error. EntityPath={args.EntityPath}; ErrorSource={args.ErrorSource}",
            Guid.Empty);
        return Task.CompletedTask;
    }

    private async Task OnMessageAsync(ProcessMessageEventArgs args)
    {
        OrderCreatedMessage payload;
        try
        {
            payload = JsonSerializer.Deserialize<OrderCreatedMessage>(args.Message.Body.ToString(), JsonOptions);
        }
        catch (JsonException ex)
        {
            _logger.LogError(
                ex,
                "Message={Message}; OrderId={OrderId}",
                "Failed to deserialize OrderCreated message body. Completing message to avoid poison retry loop.",
                Guid.Empty);
            await args.CompleteMessageAsync(args.Message);
            return;
        }

        if (payload == null)
        {
            _logger.LogError(
                "Message={Message}; OrderId={OrderId}",
                "OrderCreated message body deserialized to null. Completing message.",
                Guid.Empty);
            await args.CompleteMessageAsync(args.Message);
            return;
        }

        var propagationContext = ExtractTraceContext(args.Message.ApplicationProperties);

        using var activity = MessagingActivitySource.Instance.StartActivity(
            "OrderCreatedTopicConsumer.Process",
            ActivityKind.Consumer,
            propagationContext.ActivityContext);

        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var handler = scope.ServiceProvider.GetRequiredService<IOrderCreatedMessageHandler>();
            var result = await handler.HandleAsync(payload, args.CancellationToken);

            await args.CompleteMessageAsync(args.Message);
            _logger.LogDebug(
                "Message={Message}; OrderId={OrderId}",
                $"Completed OrderCreated message with result {result}.",
                payload.OrderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Message={Message}; OrderId={OrderId}",
                "Unexpected error processing OrderCreated message. Abandoning for retry.",
                payload.OrderId);
            await args.AbandonMessageAsync(args.Message);
        }
    }

    private static PropagationContext ExtractTraceContext(IReadOnlyDictionary<string, object> applicationProperties) =>
        Propagators.DefaultTextMapPropagator.Extract(
            default,
            applicationProperties,
            static (props, key) =>
            {
                if (!props.TryGetValue(key, out var value) || value is null)
                    return [];
                var s = value.ToString();
                return string.IsNullOrEmpty(s) ? [] : new[] { s };
            });

    public async ValueTask DisposeAsync()
    {
        if (_processor != null)
        {
            _processor.ProcessMessageAsync -= OnMessageAsync;
            _processor.ProcessErrorAsync -= OnErrorAsync;
            await _processor.DisposeAsync();
        }
    }
}
