using Jurupema.Api.Domain.Entities;
using System.Linq.Expressions;

namespace Jurupema.Api.Domain.Specifications;

public class GetProductByNameSpecification : BaseSpecification<Product>
{
    public GetProductByNameSpecification(string name)
    {
        Expression = product => product.Name == name;
    }

    public override Expression<Func<Product, bool>> GetExpression()
    {
        return Expression;
    }
}