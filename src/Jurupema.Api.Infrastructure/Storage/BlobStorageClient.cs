using Azure.Identity;
using Azure.Storage.Blobs;
using Jurupema.Api.Infrastructure.Configurations;
using Microsoft.Extensions.Options;

namespace Jurupema.Api.Infrastructure.Storage;

public class BlobStorageClient : IStorageClient
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly BlobStorageConfiguration _blobStorageConfig;

    public BlobStorageClient(IOptions<StorageConfiguration> options)
    {
        _blobStorageConfig = options.Value.BlobStorageConfig;
        _blobServiceClient = new BlobServiceClient(
            new Uri($"https://{_blobStorageConfig.StorageAccountName}.blob.core.windows.net"),
            new DefaultAzureCredential());
    }

    public async Task UploadFile(Stream file, string fileName)
    {
        await _blobServiceClient.GetBlobContainerClient(_blobStorageConfig.ContainerName).CreateIfNotExistsAsync();
        var containerClient = _blobServiceClient.GetBlobContainerClient(_blobStorageConfig.ContainerName);
        await containerClient.UploadBlobAsync(fileName, file);
    }
}
