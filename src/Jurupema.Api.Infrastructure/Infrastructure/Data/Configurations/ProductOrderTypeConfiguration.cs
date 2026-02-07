using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Jurupema.Api.Domain.Entities;
using Jurupema.Api.Domain.Enums;

namespace Jurupema.Api.Infrastructure.Data.Configurations;

public class ProductOrderTypeConfiguration : IEntityTypeConfiguration<ProductOrder>
{
    public void Configure(EntityTypeBuilder<ProductOrder> builder)
    {
        builder.ToTable("ProductOrder");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.ProductId).IsRequired();
        builder.Property(e => e.Quantity).IsRequired().HasDefaultValue(1);
        builder.Property(e => e.TotalPrice).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("GETDATE()");
        builder.Property(e => e.UpdatedAt);
        
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

public class ProductTypeConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Product");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Description).HasMaxLength(1000);
        builder.Property(e => e.Price).IsRequired().HasColumnType("decimal(18,2)");
    }
}