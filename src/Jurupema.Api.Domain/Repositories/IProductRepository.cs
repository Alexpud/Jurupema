
using Jurupema.Api.Domain.Entities;
using System.Linq.Expressions;

namespace Jurupema.Api.Domain.Repositories;

public interface IProductRepository
{
    Task<Product> GetById(int productId);
    Task<Product> GetByIdAsync<TProperty>(int productId, Expression<Func<Product, TProperty>> navigationPropertyPath) where TProperty : class;
    void InsertProductImage(ProductImage productImage);
    Task SaveChangesAsync();
}

