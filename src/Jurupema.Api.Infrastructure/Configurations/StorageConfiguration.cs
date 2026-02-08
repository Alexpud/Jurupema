namespace Jurupema.Api.Infrastructure.Configurations;


public class StorageConfiguration
{
    public const string Position = "Storage";
    public BlobStorageConfiguration BlobStorageConfig { get; set; }
}

public class BlobStorageConfiguration
{
    public string StorageAccountName { get; set; }
    public string ContainerName { get; set; }
    public bool Enabled { get; set; }
}
