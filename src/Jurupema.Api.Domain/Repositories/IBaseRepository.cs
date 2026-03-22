using System.Linq.Expressions;

namespace Jurupema.Api.Domain.Repositories;

public interface IBaseRepository<T> where T : BaseEntity
{
    void Insert(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<List<T>> GetAllAsync();
    Task<T> GetByIdAsync(int id, Expression<Func<T, object>> include = null);
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate);
    Task<int> SaveChangesAsync();
}
