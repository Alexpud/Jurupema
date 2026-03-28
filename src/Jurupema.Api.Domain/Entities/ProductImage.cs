using Jurupema.Api.Domain;

namespace Jurupema.Api.Domain.Entities;

public class ProductImage(Guid productId, string name) : BaseEntity
{
    public const int MaxNameLength = 200;

    internal static string NormalizeName(string value, string paramName) =>
        Guards.ThrowIfNullWhiteSpaceOrTooLong(value, paramName, MaxNameLength);

    public string Name { get; private set; } = NormalizeName(name, nameof(name));
    public Guid ProductId { get; private set; } = Guards.ThrowIfDefault(productId, nameof(productId));
    public Product Product { get; private set; }
}
