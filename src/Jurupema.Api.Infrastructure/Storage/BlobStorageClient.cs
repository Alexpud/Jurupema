using Azure.Identity;
using Azure.Storage.Blobs;

namespace Jurupema.Api.Infrastructure.Storage;

public class BlobStorageClient : IStorageClient
{
    private readonly BlobServiceClient _blobServiceClient;
    public BlobStorageClient()
    {
        _blobServiceClient = new BlobServiceClient(
            new Uri("https://stjurupemabrazil.blob.core.windows.net"),
            new DefaultAzureCredential());
    }

    public async Task UploadFile(Stream file, string fileName)
    {
        var containerExists = await _blobServiceClient.GetBlobContainerClient("files-container").ExistsAsync();
        if (!containerExists) 
            await _blobServiceClient.CreateBlobContainerAsync("files-container");

        var containerClient = _blobServiceClient.GetBlobContainerClient("files-container");
        await containerClient.UploadBlobAsync(fileName, file);
    }
}
