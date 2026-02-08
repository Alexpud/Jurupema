using Jurupema.Api.Infrastructure.Configurations;
using Jurupema.Api.Infrastructure.Data;
using Jurupema.Api.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Jurupema.Api.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        services.Configure<StorageConfiguration>(configuration.GetSection(StorageConfiguration.Position));
        services.AddSingleton<IStorageClient, BlobStorageClient>();
        return services;
    }
}
