
namespace Jurupema.Api.Domain;

public static class Guards
{
    public static Guid ThrowIfDefault(Guid value, string paramName) =>
        value == Guid.Empty ? throw new ArgumentException($"{paramName} must not be empty.", paramName) : value;

    public static decimal ThrowIfNegative(decimal value, string paramName) =>
        value < 0 ? throw new ArgumentOutOfRangeException(paramName, value, $"{paramName} must not be negative.") : value;

    public static int ThrowIfNegative(int value, string paramName) =>
        value < 0 ? throw new ArgumentOutOfRangeException(paramName, value, $"{paramName} must not be negative.") : value;

    public static int ThrowIfLessThanOrEqualToZero(int value, string paramName) =>
        value <= 0 ? throw new ArgumentOutOfRangeException(paramName, value, $"{paramName} must be greater than zero.") : value;

    public static string ThrowIfEmpty(string value, string paramName) =>
        string.IsNullOrEmpty(value) ? throw new ArgumentException($"{paramName} must not be empty.", paramName) : value;

    public static string ThrowIfNullWhiteSpaceOrTooLong(string value, string paramName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{paramName} must not be null or whitespace.", paramName);
        var trimmed = value.Trim();
        if (trimmed.Length > maxLength)
            throw new ArgumentException($"{paramName} must not exceed {maxLength} characters.", paramName);
        return trimmed;
    }

    public static string ThrowIfLengthExceeds(string value, string paramName, int maxLength) =>
        value.Length > maxLength
            ? throw new ArgumentOutOfRangeException(paramName, value.Length, $"{paramName} must not exceed {maxLength} characters.")
            : value;
}
