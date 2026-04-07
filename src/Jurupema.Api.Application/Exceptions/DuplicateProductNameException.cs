namespace Jurupema.Api.Application.Exceptions;

public sealed class DuplicateProductNameException : DomainException
{
    public DuplicateProductNameException(string name)
        : base("duplicate_product_name", $"A product with the name '{name}' already exists.")
    {
        Name = name;
    }

    public string Name { get; }
}
