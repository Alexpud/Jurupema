namespace Jurupema.Api.Application.Exceptions;

public sealed class DuplicateProductImageException : DomainException
{
    public DuplicateProductImageException(string fileName)
        : base("duplicate_product_image", $"The product already has an image for '{fileName}'.")
    {
        FileName = fileName;
    }

    public string FileName { get; }
}
