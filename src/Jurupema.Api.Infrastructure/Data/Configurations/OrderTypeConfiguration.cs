using Jurupema.Api.Domain.Entities;
using Jurupema.Api.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jurupema.Api.Infrastructure.Data.Configurations;

public class OrderTypeConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Order");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.TotalPrice).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(e => e.CreatedAt).IsRequired();

        builder.Property(e => e.Status)
            .IsRequired()
            .HasDefaultValue(ProductOrderStatus.Pending)
            .HasConversion<string>();

        builder.Property(e => e.PaymentMethod)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(e => e.PaymentStatus)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(e => e.PaymentLink).IsRequired().HasMaxLength(200);
        builder.Property(e => e.PaymentLinkExpiration).IsRequired().HasMaxLength(200);
        builder.Property(e => e.PaymentLinkQrCode).IsRequired().HasMaxLength(200);
        builder.Property(e => e.PaymentLinkQrCodeExpiration).IsRequired().HasMaxLength(200);
    }
}
