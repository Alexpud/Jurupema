namespace Jurupema.Api.Application.Messaging;

public interface IOrderCreatedMessageHandler
{
    Task<OrderCreatedMessageProcessingResult> HandleAsync(OrderCreatedMessage message, CancellationToken cancellationToken = default);
}
