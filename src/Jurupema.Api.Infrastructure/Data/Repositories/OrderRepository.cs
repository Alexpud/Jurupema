using Jurupema.Api.Domain.Entities;
using Jurupema.Api.Domain.Repositories;

namespace Jurupema.Api.Infrastructure.Data.Repositories;

public class OrderRepository : BaseRepository<Order>, IOrderRepository
{
    public OrderRepository(JurupemaDbContext context) : base(context)
    {
    }
}
