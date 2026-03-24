using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Jurupema.Api.Application.Storage;
using Jurupema.Api.Infrastructure.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Jurupema.Api.Infrastructure.Storage;

public class BlobStorageClient : IStorageClient
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly BlobStorageConfiguration _blobStorageConfig;
    private readonly ILogger<BlobStorageClient> _logger;

    public BlobStorageClient(IOptions<StorageConfiguration> options, ILogger<BlobStorageClient> logger)
    {
        _blobStorageConfig = options.Value.BlobStorageConfig;
        _logger = logger;
        var blobUri = new Uri($"https://{_blobStorageConfig.StorageAccountName}.blob.core.windows.net");
        _blobServiceClient = new BlobServiceClient(blobUri, new DefaultAzureCredential());
    }

    public async Task UploadFileAsync(Stream file, string fileName)
    {
        await _blobServiceClient.GetBlobContainerClient(_blobStorageConfig.ContainerName).CreateIfNotExistsAsync();
        var containerClient = _blobServiceClient.GetBlobContainerClient(_blobStorageConfig.ContainerName);
        await containerClient.UploadBlobAsync(fileName, file);
    }

    public async Task DeleteFileAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_blobStorageConfig.ContainerName);
        var blobClient = containerClient.GetBlobClient(fileName);
        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    public async Task<string> GetTemporaryReadUrlAsync(string fileName, TimeSpan lifetime, CancellationToken cancellationToken = default)
    {
        if (!_blobStorageConfig.Enabled)
            return null;

        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_blobStorageConfig.ContainerName);
            var blobClient = containerClient.GetBlobClient(fileName);
            var accountName = _blobStorageConfig.StorageAccountName ?? _blobServiceClient.AccountName;

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _blobStorageConfig.ContainerName,
                BlobName = fileName,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5),
                ExpiresOn = DateTimeOffset.UtcNow.Add(lifetime)
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var keyStartsOn = DateTimeOffset.UtcNow.AddMinutes(-5);
            var keyExpiresOn = DateTimeOffset.UtcNow.AddDays(1);
            var userDelegationKey = await _blobServiceClient.GetUserDelegationKeyAsync(
                keyStartsOn,
                keyExpiresOn,
                cancellationToken);
            var sas = sasBuilder.ToSasQueryParameters(userDelegationKey, accountName);

            var uriBuilder = new BlobUriBuilder(blobClient.Uri) { Sas = sas };
            return uriBuilder.ToUri().AbsoluteUri;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to generate temporary read SAS URL for blob {FileName}", fileName);
            return null;
        }
    }
}
