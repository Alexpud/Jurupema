
namespace Jurupema.Api.Infrastructure.Files;

public interface IStorageClient
{
    Task UploadFile(Stream file, string fileName);
}