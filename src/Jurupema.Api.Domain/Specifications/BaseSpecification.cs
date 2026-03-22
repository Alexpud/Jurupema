using System.Linq.Expressions;

namespace Jurupema.Api.Domain.Specifications;

public abstract class BaseSpecification<T> : ISpecification<T>
{
    protected Expression<Func<T, bool>> Expression { get; set; }
    public abstract Expression<Func<T, bool>> GetExpression();
}
