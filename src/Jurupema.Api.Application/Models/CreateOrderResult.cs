using Jurupema.Api.Domain.Entities;
using Jurupema.Api.Domain.Enums;

namespace Jurupema.Api.Application.Models;

public record CreateOrderPaymentResult(
    PaymentMethod PaymentMethod,
    PaymentStatus PaymentStatus,
    string PaymentLink,
    string PaymentLinkExpiration,
    string PaymentLinkQrCode,
    string PaymentLinkQrCodeExpiration);

public record CreateOrderResult(
    Guid OrderId,
    decimal TotalPrice,
    IReadOnlyList<CreateOrderLineResult> Lines,
    CreateOrderPaymentResult Payment)
{
    public static CreateOrderResult FromOrder(Order order, IReadOnlyList<CreateOrderLineItem> requestLines)
    {
        var lineResults = new List<CreateOrderLineResult>(requestLines.Count);
        for (var i = 0; i < requestLines.Count; i++)
        {
            var persisted = order.ProductOrders[i];
            var requested = requestLines[i];
            lineResults.Add(new CreateOrderLineResult(
                requested.ProductId,
                requested.Quantity,
                persisted.Price));
        }

        var payment = new CreateOrderPaymentResult(
            order.PaymentMethod,
            order.PaymentStatus,
            order.PaymentLink,
            order.PaymentLinkExpiration,
            order.PaymentLinkQrCode,
            order.PaymentLinkQrCodeExpiration);

        return new CreateOrderResult(order.Id, order.TotalPrice, lineResults, payment);
    }
}

public record CreateOrderLineResult(Guid ProductId, int Quantity, decimal LineTotal);
