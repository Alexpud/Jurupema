using Jurupema.Api.Infrastructure.Configurations;
using Jurupema.Api.Infrastructure.Data;
using Jurupema.Api.Infrastructure.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Jurupema.Api.Infrastructure;

public static class ServiceExtensions
{
    public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("jurupema-db")));
        builder.Services.Configure<StorageConfiguration>(builder.Configuration.GetSection(StorageConfiguration.Position));
        builder.Services.AddSingleton<IStorageClient, BlobStorageClient>();
        builder.AddServiceDefaults();
        
        return builder;
    }
}
