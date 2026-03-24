namespace Jurupema.Api.Application.Storage;

public interface IStorageClient
{
    Task UploadFileAsync(Stream file, string fileName);

    /// <summary>
    /// Returns a time-limited read URL for the blob (e.g. SAS), or null when cloud temporary URLs are not available.
    /// </summary>
    Task<string> GetTemporaryReadUrlAsync(string fileName, TimeSpan lifetime, CancellationToken cancellationToken = default);
}