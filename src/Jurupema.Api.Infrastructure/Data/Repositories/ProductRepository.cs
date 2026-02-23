using Jurupema.Api.Domain.Entities;
using Jurupema.Api.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Jurupema.Api.Infrastructure.Data.Repositories;

public class ProductRepository(JurupemaDbContext context) : IProductRepository
{
    public async Task<Product> GetByIdAsync<TProperty>(int productId, Expression<Func<Product, TProperty>> navigationPropertyPath) where TProperty: class
    {
        return await context.Products.Where(p => p.Id == productId)
            .Include(p => p.ProductImages)
            .FirstOrDefaultAsync();
    }

    public async Task<Product> GetById(int productId)
        => await context.Products.FindAsync(productId);

    public void InsertProductImage(ProductImage productImage)
    {
        context.ProductImages.Add(productImage);
    }

    public async Task SaveChangesAsync()
        => await context.SaveChangesAsync();
}