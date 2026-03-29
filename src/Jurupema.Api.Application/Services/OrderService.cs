using Jurupema.Api.Application.Exceptions;
using Jurupema.Api.Application.Messaging;
using Jurupema.Api.Application.Models;
using Jurupema.Api.Domain.Entities;
using Jurupema.Api.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Jurupema.Api.Application.Services;

public class OrderService(
    IProductRepository productRepository,
    IOrderRepository orderRepository,
    ITopicMessagePublisher topicMessagePublisher,
    IOrderServiceBusTopics orderServiceBusTopics,
    ILogger<OrderService> logger)
{
    public async Task<CreateOrderResult> CreateOrderAsync(CreateOrderParameter parameter, CancellationToken cancellationToken = default)
    {
        var distinctProductIds = parameter.Products.Select(l => l.ProductId).Distinct().ToList();
        var products = await productRepository.GetByIdsAsync(distinctProductIds, cancellationToken);
        var registeredProductIds = products.ToDictionary(p => p.Id);

        foreach (var line in parameter.Products)
        {
            if (!registeredProductIds.ContainsKey(line.ProductId))
                throw new ProductNotFoundException(line.ProductId);
        }

        var order = new Order(
            parameter.Payment.PaymentMethod,
            parameter.Payment.PaymentStatus,
            parameter.Payment.PaymentLink,
            parameter.Payment.PaymentLinkExpiration,
            parameter.Payment.PaymentLinkQrCode,
            parameter.Payment.PaymentLinkQrCodeExpiration);

        foreach (var orderProduct in parameter.Products)
        {
            var product = registeredProductIds[orderProduct.ProductId];
            order.AddLine(product, orderProduct.Quantity);
        }

        orderRepository.Insert(order);
        await orderRepository.SaveChangesAsync();

        logger.LogInformation(
            "Message={Message}; OrderId={OrderId}",
            "Order created; publishing OrderCreated message.",
            order.Id);

        await topicMessagePublisher.PublishAsync(
            orderServiceBusTopics.OrderCreated,
            "OrderCreatedMessage",
            new OrderCreatedMessage(order.Id),
            cancellationToken);

        return CreateOrderResult.FromOrder(order, parameter.Products);
    }
}
