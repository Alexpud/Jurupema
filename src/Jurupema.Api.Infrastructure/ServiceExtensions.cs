using FluentValidation;
using Jurupema.Api.Application.Models;
using Jurupema.Api.Application.Products;
using Jurupema.Api.Application.Storage;
using Jurupema.Api.Domain.Repositories;
using Jurupema.Api.Infrastructure.Configurations;
using Jurupema.Api.Infrastructure.Data;
using Jurupema.Api.Infrastructure.Data.Repositories;
using Jurupema.Api.Infrastructure.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Jurupema.Api.Infrastructure;

public static class ServiceExtensions
{
    public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
    {
        // Infra
        builder.Services.AddDbContext<JurupemaDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("database")));
        builder.Services.Configure<StorageConfiguration>(builder.Configuration.GetSection(StorageConfiguration.Position));
        builder.Services.AddSingleton<IStorageClient, BlobStorageClient>();
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.AddServiceDefaults();

        // Application
        builder.Services.AddScoped<ProductService>();
        builder.Services.AddScoped<ProductImageService>();
        builder.Services.AddValidatorsFromAssemblyContaining<CreateProductParameterValidator>();

        return builder;
    }

    public static async Task RunMigrationsAsync(this WebApplication builder)
    {
        using var scope = builder.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<JurupemaDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}
