using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jurupema.Api.Domain.Enums;

namespace Jurupema.Api.Domain.Entities;

public class ProductOrder
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public int ProductId { get; private set; }
    public Product Product { get; set; }
    public int Quantity { get; private set; }
    public decimal TotalPrice { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }
    public ProductOrderStatus Status { get; set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public PaymentStatus PaymentStatus { get; private set; }
    public string PaymentLink { get; private set; }
    public string PaymentLinkExpiration { get; private set; }
    public string PaymentLinkQrCode { get; private set; }
    public string PaymentLinkQrCodeExpiration { get; private set; }
}
