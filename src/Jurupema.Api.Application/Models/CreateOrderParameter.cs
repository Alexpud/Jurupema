using FluentValidation;
using Jurupema.Api.Domain.Enums;

namespace Jurupema.Api.Application.Models;

public class CreateOrderParameter
{
    public List<CreateOrderLineItem> Products { get; set; } = [];
    public CreateOrderPaymentInfo Payment { get; set; } = new();
}

public class CreateOrderLineItem
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}

public class CreateOrderPaymentInfo
{
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public string PaymentLink { get; set; } = string.Empty;
    public string PaymentLinkExpiration { get; set; } = string.Empty;
    public string PaymentLinkQrCode { get; set; } = string.Empty;
    public string PaymentLinkQrCodeExpiration { get; set; } = string.Empty;
}

public class CreateOrderPaymentInfoValidator : AbstractValidator<CreateOrderPaymentInfo>
{
    public CreateOrderPaymentInfoValidator()
    {
        RuleFor(x => x.PaymentMethod).IsInEnum();
        RuleFor(x => x.PaymentStatus)
            .Equal(PaymentStatus.Pending)
            .WithMessage("New orders must use payment status Pending.");
        RuleFor(x => x.PaymentLink).MaximumLength(200);
        RuleFor(x => x.PaymentLinkExpiration).MaximumLength(200);
        RuleFor(x => x.PaymentLinkQrCode).MaximumLength(200);
        RuleFor(x => x.PaymentLinkQrCodeExpiration).MaximumLength(200);
    }
}

public class CreateOrderLineItemValidator : AbstractValidator<CreateOrderLineItem>
{
    public CreateOrderLineItemValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}

public class CreateOrderParameterValidator : AbstractValidator<CreateOrderParameter>
{
    public CreateOrderParameterValidator()
    {
        RuleFor(x => x.Products).NotEmpty();
        RuleForEach(x => x.Products).SetValidator(new CreateOrderLineItemValidator());
        RuleFor(x => x.Payment).NotNull().SetValidator(new CreateOrderPaymentInfoValidator());
    }
}
