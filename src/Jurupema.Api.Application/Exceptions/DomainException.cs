namespace Jurupema.Api.Application.Exceptions;

public abstract class DomainException : Exception
{
    protected DomainException(string code, string message)
        : base(message)
    {
        Code = string.IsNullOrWhiteSpace(code)
            ? throw new ArgumentException("Domain exception code must be provided.", nameof(code))
            : code;
    }

    public string Code { get; }
}

