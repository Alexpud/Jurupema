namespace Jurupema.Api.Infrastructure.Storage;

public interface IStorageClient
{
    Task UploadFile(Stream file, string fileName);
}