namespace Jurupema.Api.Application.Storage;

public interface IStorageClient
{
    Task UploadFileAsync(Stream file, string fileName);
}