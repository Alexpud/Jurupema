using Jurupema.Api.Domain;
using Jurupema.Api.Domain.Enums;

namespace Jurupema.Api.Domain.Entities;

public class Order(
    PaymentMethod paymentMethod,
    PaymentStatus paymentStatus,
    string paymentLink,
    string paymentLinkExpiration,
    string paymentLinkQrCode,
    string paymentLinkQrCodeExpiration) : BaseEntity
{
    private const int MaxPaymentFieldLength = 200;

    private static string PaymentField(string value, string paramName)
    {
        var s = value is null ? string.Empty : value;
        return Guards.ThrowIfLengthExceeds(s, paramName, MaxPaymentFieldLength);
    }

    public List<ProductOrder> ProductOrders { get; private set; } = [];
    public decimal TotalPrice { get; private set; }
    public ProductOrderStatus Status { get; private set; } = ProductOrderStatus.Pending;
    public PaymentMethod PaymentMethod { get; private set; } = paymentMethod;
    public PaymentStatus PaymentStatus { get; private set; } = paymentStatus;
    public string PaymentLink { get; private set; } = PaymentField(paymentLink, nameof(paymentLink));
    public string PaymentLinkExpiration { get; private set; } = PaymentField(paymentLinkExpiration, nameof(paymentLinkExpiration));
    public string PaymentLinkQrCode { get; private set; } = PaymentField(paymentLinkQrCode, nameof(paymentLinkQrCode));
    public string PaymentLinkQrCodeExpiration { get; private set; } = PaymentField(paymentLinkQrCodeExpiration, nameof(paymentLinkQrCodeExpiration));

    public void AddLine(Product product, int quantity)
    {
        ArgumentNullException.ThrowIfNull(product);
        Guards.ThrowIfLessThanOrEqualToZero(quantity, nameof(quantity));

        var lineTotal = product.Price * quantity;
        var line = new ProductOrder(Id, product.Id, lineTotal)
        {
            Order = this,
            Product = product
        };
        ProductOrders.Add(line);
        RecalculateTotal();
    }

    private void RecalculateTotal() =>
        TotalPrice = ProductOrders.Sum(l => l.Price);
}
