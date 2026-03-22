using Jurupema.Api.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Jurupema.Api.Infrastructure.Data.Repositories;

public abstract class BaseRepository<T>(JurupemaDbContext context) : IBaseRepository<T> where T : BaseEntity
{
    private readonly JurupemaDbContext context = context;
    protected DbSet<T> DbSet => context.Set<T>();

    public void Delete(T entity) => context.Set<T>().Remove(entity);

    public async Task<List<T>> GetAllAsync() => await context.Set<T>().ToListAsync();

    public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate) =>
        await context.Set<T>().Where(predicate).ToListAsync();

    public async Task<T> GetByIdAsync(int id, Expression<Func<T, object>> include = null)
    {
        var query = context.Set<T>().AsQueryable();
        if (include != null)
            query = query.Include(include);
        return await query.FirstOrDefaultAsync(e => e.Id == id);
    }

    public void Insert(T entity) => context.Set<T>().Add(entity);

    public void Update(T entity) => context.Set<T>().Update(entity);

    public async Task<int> SaveChangesAsync() => await context.SaveChangesAsync();
}   