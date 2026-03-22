using System.Linq.Expressions;

namespace Jurupema.Api.Domain.Specifications;

public interface ISpecification<T>
{
    Expression<Func<T, bool>> GetExpression();
}
