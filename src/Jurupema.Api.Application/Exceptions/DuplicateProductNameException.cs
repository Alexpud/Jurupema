namespace Jurupema.Api.Application.Exceptions;

public sealed class DuplicateProductNameException : Exception
{
    public DuplicateProductNameException(string name)
        : base($"A product with the name '{name}' already exists.")
    {
        Name = name;
    }

    public string Name { get; }
}
