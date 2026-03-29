using System.Linq.Expressions;
using Jurupema.Api.Application.Messaging;
using Jurupema.Api.Application.Services.Orders;
using Jurupema.Api.Domain.Entities;
using Jurupema.Api.Domain.Enums;
using Jurupema.Api.Domain.Repositories;
using Moq;
using Moq.AutoMock;

namespace Jurupema.Api.Application.Tests.Orders;

public class OrderCreatedMessageHandlerTests
{
    private readonly AutoMocker _autoMocker;
    private readonly OrderCreatedMessageHandler _sut;

    public OrderCreatedMessageHandlerTests()
    {
        _autoMocker = new AutoMocker();
        _sut = _autoMocker.CreateInstance<OrderCreatedMessageHandler>();
    }

    [Fact]
    public async Task HandleAsync_when_order_exists_marks_paid_updates_and_saves()
    {
        // Arrange
        var orderId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var order = new Order(
            PaymentMethod.Cash,
            PaymentStatus.Pending,
            "",
            "",
            "",
            "");
        typeof(BaseEntity).GetProperty(nameof(BaseEntity.Id))!.SetValue(order, orderId);

        _autoMocker.GetMock<IOrderRepository>()
            .Setup(r => r.GetByIdAsync(orderId, It.IsAny<Expression<Func<Order, object>>>()))
            .ReturnsAsync(order);

        var message = new OrderCreatedMessage(orderId);

        // Act
        var result = await _sut.HandleAsync(message, CancellationToken.None);

        // Assert
        Assert.Equal(OrderCreatedMessageProcessingResult.OrderMarkedPaid, result);
        Assert.Equal(ProductOrderStatus.Paid, order.Status);
        _autoMocker.GetMock<IOrderRepository>().Verify(r => r.Update(order), Times.Once);
        _autoMocker.GetMock<IOrderRepository>().Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_when_order_missing_returns_not_found_and_does_not_save()
    {
        // Arrange
        var orderId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
        _autoMocker.GetMock<IOrderRepository>()
            .Setup(r => r.GetByIdAsync(orderId, It.IsAny<Expression<Func<Order, object>>>()))
            .ReturnsAsync((Order)null!);

        var message = new OrderCreatedMessage(orderId);

        // Act
        var result = await _sut.HandleAsync(message, CancellationToken.None);

        // Assert
        Assert.Equal(OrderCreatedMessageProcessingResult.OrderNotFound, result);
        _autoMocker.GetMock<IOrderRepository>().Verify(r => r.Update(It.IsAny<Order>()), Times.Never);
        _autoMocker.GetMock<IOrderRepository>().Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}
