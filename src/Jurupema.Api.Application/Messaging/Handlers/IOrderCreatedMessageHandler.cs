using Jurupema.Api.Application.Models;

namespace Jurupema.Api.Application.Messaging.Handlers;

public interface IOrderCreatedMessageHandler
{
    Task<OrderCreatedMessageProcessingResult> HandleAsync(OrderCreatedMessage message, CancellationToken cancellationToken = default);
}
