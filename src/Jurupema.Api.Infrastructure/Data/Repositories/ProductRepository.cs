using Jurupema.Api.Domain.Entities;
using Jurupema.Api.Domain.Repositories;

namespace Jurupema.Api.Infrastructure.Data.Repositories;

public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(JurupemaDbContext context) : base(context)
    {
    }
}

