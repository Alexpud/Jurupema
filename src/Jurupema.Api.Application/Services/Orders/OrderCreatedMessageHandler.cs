using Jurupema.Api.Application.Messaging;
using Jurupema.Api.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Jurupema.Api.Application.Services.Orders;

public class OrderCreatedMessageHandler(
    IOrderRepository orderRepository,
    ILogger<OrderCreatedMessageHandler> logger) : IOrderCreatedMessageHandler
{
    public async Task<OrderCreatedMessageProcessingResult> HandleAsync(
        OrderCreatedMessage message,
        CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetByIdAsync(message.OrderId);
        if (order is null)
        {
            logger.LogWarning(
                "Message={Message}; OrderId={OrderId}",
                "Order not found for OrderCreated message. Completing message without retry.",
                message.OrderId);
            return OrderCreatedMessageProcessingResult.OrderNotFound;
        }

        order.MarkAsPaid();
        orderRepository.Update(order);
        await orderRepository.SaveChangesAsync();

        logger.LogInformation(
            "Message={Message}; OrderId={OrderId}",
            "Order marked as Paid after OrderCreated message.",
            message.OrderId);

        return OrderCreatedMessageProcessingResult.OrderMarkedPaid;
    }
}
