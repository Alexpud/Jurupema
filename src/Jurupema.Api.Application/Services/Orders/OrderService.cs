using Jurupema.Api.Application.Exceptions;
using Jurupema.Api.Application.Models;
using Jurupema.Api.Domain.Entities;
using Jurupema.Api.Domain.Repositories;

namespace Jurupema.Api.Application.Services.Orders;

public class OrderService(IProductRepository productRepository, IOrderRepository orderRepository)
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

        return CreateOrderResult.FromOrder(order, parameter.Products);
    }
}
