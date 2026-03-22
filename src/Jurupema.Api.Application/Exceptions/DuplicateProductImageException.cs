namespace Jurupema.Api.Application.Exceptions;

public sealed class DuplicateProductImageException : Exception
{
    public DuplicateProductImageException(string fileName)
        : base($"The product already has an image for '{fileName}'.")
    {
        FileName = fileName;
    }

    public string FileName { get; }
}
