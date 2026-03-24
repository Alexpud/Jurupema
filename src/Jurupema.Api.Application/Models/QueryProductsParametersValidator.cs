using FluentValidation;

namespace Jurupema.Api.Application.Models;

public class QueryProductsParametersValidator : AbstractValidator<QueryProductsParameters>
{
    public QueryProductsParametersValidator()
    {
        RuleFor(x => x.PageIndex).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
        RuleFor(x => x.NameFilter).MaximumLength(500).When(x => x.NameFilter != null);
        RuleFor(x => x.SortBy).IsInEnum();
        RuleFor(x => x.SortDirection).IsInEnum();
    }
}
