using Jurupema.Api.Application.Messaging;
using Jurupema.Api.Application.Models;
using Jurupema.Api.Application.Services.Orders;
using Jurupema.Api.Domain.Entities;
using Jurupema.Api.Domain.Enums;
using Jurupema.Api.Infrastructure.Data;
using Jurupema.Api.Infrastructure.Data.Repositories;
using Jurupema.Api.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace Jurupema.Api.Infrastructure.Tests.Orders;

public class OrderServicePersistenceTests
{
    [Fact]
    public async Task CreateOrderAsync_persists_order_and_lines_with_snapshot_totals()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<JurupemaDbContext>()
            .UseInMemoryDatabase($"orders-{Guid.NewGuid():N}")
            .Options;

        await using var context = new JurupemaDbContext(options);
        await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

        var productRepository = new ProductRepository(context);
        var orderRepository = new OrderRepository(context);
        var sut = new OrderService(
            productRepository,
            orderRepository,
            new NoOpTopicMessagePublisher(),
            new FixedOrderServiceBusTopics(),
            NullLogger<OrderService>.Instance);

        var productA = new Product("A", "da", 4m);
        var productB = new Product("B", "db", 2.5m);
        productRepository.Insert(productA);
        productRepository.Insert(productB);
        await productRepository.SaveChangesAsync();

        var request = new CreateOrderParameter
        {
            Products =
            [
                new CreateOrderLineItem { ProductId = productA.Id, Quantity = 3 },
                new CreateOrderLineItem { ProductId = productB.Id, Quantity = 2 }
            ],
            Payment = new CreateOrderPaymentInfo
            {
                PaymentMethod = PaymentMethod.BankTransfer,
                PaymentStatus = PaymentStatus.Pending
            }
        };

        // Act
        var result = await sut.CreateOrderAsync(request, TestContext.Current.CancellationToken);

        // Assert
        var orderEntity = await context.Orders
            .Include(o => o.ProductOrders)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == result.OrderId, TestContext.Current.CancellationToken);

        Assert.NotNull(orderEntity);
        Assert.Multiple(
            () => Assert.Equal(17m, orderEntity.TotalPrice),
            () => Assert.Equal(2, orderEntity.ProductOrders.Count),
            () => Assert.Equal(12m, orderEntity.ProductOrders.Single(l => l.ProductId == productA.Id).Price),
            () => Assert.Equal(5m, orderEntity.ProductOrders.Single(l => l.ProductId == productB.Id).Price),
            () => Assert.Equal(17m, result.TotalPrice),
            () => Assert.Equal(2, result.Lines.Count),
            () => Assert.Equal(PaymentMethod.BankTransfer, orderEntity.PaymentMethod),
            () => Assert.Equal(PaymentStatus.Pending, orderEntity.PaymentStatus),
            () => Assert.Equal(PaymentMethod.BankTransfer, result.Payment.PaymentMethod));
    }

    private sealed class FixedOrderServiceBusTopics : IOrderServiceBusTopics
    {
        public string OrderCreated => "sbt-jurupema-order-created";
    }
}
