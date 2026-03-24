using Jurupema.Api.Domain.Entities;

namespace Jurupema.Api.Application.Models;

public record ProductImageListItemResult
{
    public int Id { get; }
    public string Name { get; }
    public string Url { get; }

    /// <param name="accessUrl">Time-limited read URL from storage, or null when not using cloud / unavailable.</param>
    public ProductImageListItemResult(ProductImage image, string accessUrl)
    {
        Id = image.Id;
        Name = image.Name;
        Url = accessUrl;
    }
}
