using Jurupema.Api.Application.Exceptions;
using Jurupema.Api.Application.Models;
using Jurupema.Api.Application.Services.Orders;
using Jurupema.Api.Application.Tests.Builders;
using Jurupema.Api.Domain.Entities;
using Jurupema.Api.Domain.Enums;
using Jurupema.Api.Domain.Repositories;
using Moq;
using Moq.AutoMock;

namespace Jurupema.Api.Application.Tests.Orders;

public class OrderServiceTests
{
    private readonly AutoMocker _autoMocker;
    private readonly OrderService _sut;

    public OrderServiceTests()
    {
        _autoMocker = new AutoMocker();
        _sut = _autoMocker.CreateInstance<OrderService>();
    }

    [Fact]
    public async Task CreateOrderAsync_when_product_not_found_throws()
    {
        // Arrange
        var missingId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");
        _autoMocker.GetMock<IProductRepository>()
            .Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<Product>)[]);

        var request = new CreateOrderParameter
        {
            Products = [new CreateOrderLineItem { ProductId = missingId, Quantity = 1 }],
            Payment = new CreateOrderPaymentInfo
            {
                PaymentMethod = PaymentMethod.Cash,
                PaymentStatus = PaymentStatus.Pending
            }
        };

        // Act
        var act = () => _sut.CreateOrderAsync(request, CancellationToken.None);

        // Assert
        var ex = await Assert.ThrowsAsync<ProductNotFoundException>(act);
        Assert.Equal(missingId, ex.ProductId);
    }

    [Fact]
    public async Task CreateOrderAsync_when_valid_inserts_order_with_lines_and_saves()
    {
        // Arrange
        var productA = new ProductBuilder().WithPrice(10m).Build();
        var productB = new ProductBuilder().WithPrice(5m).Build();

        _autoMocker.GetMock<IProductRepository>()
            .Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<Product>)new List<Product> { productA, productB });

        Order captured = null!;
        _autoMocker.GetMock<IOrderRepository>()
            .Setup(r => r.Insert(It.IsAny<Order>()))
            .Callback<Order>(o => captured = o);

        var request = new CreateOrderParameter
        {
            Products =
            [
                new CreateOrderLineItem { ProductId = productA.Id, Quantity = 2 },
                new CreateOrderLineItem { ProductId = productB.Id, Quantity = 1 }
            ],
            Payment = new CreateOrderPaymentInfo
            {
                PaymentMethod = PaymentMethod.Pix,
                PaymentStatus = PaymentStatus.Pending,
                PaymentLink = "https://pay.example/pix",
                PaymentLinkExpiration = "2026-12-31",
                PaymentLinkQrCode = "qr-data",
                PaymentLinkQrCodeExpiration = "2026-12-31"
            }
        };

        // Act
        var result = await _sut.CreateOrderAsync(request, CancellationToken.None);

        // Assert
        Assert.Multiple(
            () => _autoMocker.GetMock<IOrderRepository>().Verify(r => r.Insert(It.IsAny<Order>()), Times.Once),
            () => _autoMocker.GetMock<IOrderRepository>().Verify(r => r.SaveChangesAsync(), Times.Once),
            () => Assert.NotNull(captured),
            () => Assert.Equal(2, captured.ProductOrders.Count),
            () => Assert.Equal(25m, captured.TotalPrice),
            () => Assert.Equal(productA.Id, captured.ProductOrders[0].ProductId),
            () => Assert.Equal(20m, captured.ProductOrders[0].Price),
            () => Assert.Equal(productB.Id, captured.ProductOrders[1].ProductId),
            () => Assert.Equal(5m, captured.ProductOrders[1].Price),
            () => Assert.Equal(captured.Id, result.OrderId),
            () => Assert.Equal(25m, result.TotalPrice),
            () => Assert.Equal(2, result.Lines.Count),
            () => Assert.Equal(20m, result.Lines[0].LineTotal),
            () => Assert.Equal(5m, result.Lines[1].LineTotal),
            () => Assert.Equal(PaymentMethod.Pix, captured.PaymentMethod),
            () => Assert.Equal(PaymentStatus.Pending, captured.PaymentStatus),
            () => Assert.Equal("https://pay.example/pix", captured.PaymentLink),
            () => Assert.Equal(PaymentMethod.Pix, result.Payment.PaymentMethod),
            () => Assert.Equal(PaymentStatus.Pending, result.Payment.PaymentStatus));
    }
}
