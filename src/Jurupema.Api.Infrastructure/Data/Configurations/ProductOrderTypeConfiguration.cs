using Jurupema.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jurupema.Api.Infrastructure.Data.Configurations;

public class ProductOrderTypeConfiguration : IEntityTypeConfiguration<ProductOrder>
{
    public void Configure(EntityTypeBuilder<ProductOrder> builder)
    {
        builder.ToTable("ProductOrder");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.OrderId).IsRequired();
        builder.Property(e => e.ProductId).IsRequired();
        builder.Property(e => e.Price).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(e => e.CreatedAt).IsRequired();

        builder.HasOne(e => e.Order)
            .WithMany(o => o.ProductOrders)
            .HasForeignKey(e => e.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Product)
            .WithMany(p => p.ProductOrders)
            .HasForeignKey(e => e.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
