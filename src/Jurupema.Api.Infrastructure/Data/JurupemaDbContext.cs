using Jurupema.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Jurupema.Api.Infrastructure.Data;

public class JurupemaDbContext : DbContext
{
    public JurupemaDbContext(DbContextOptions<JurupemaDbContext> options) 
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<ProductOrder> ProductOrders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(JurupemaDbContext).Assembly);
    }
}
