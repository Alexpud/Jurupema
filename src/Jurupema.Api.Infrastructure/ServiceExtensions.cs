using Azure.Messaging.ServiceBus;
using FluentValidation;
using Jurupema.Api.Application.Messaging;
using Jurupema.Api.Application.Messaging.Handlers;
using Jurupema.Api.Application.Models;
using Jurupema.Api.Application.Services;
using Jurupema.Api.Application.Storage;
using Jurupema.Api.Domain.Repositories;
using Jurupema.Api.Infrastructure.Configurations;
using Jurupema.Api.Infrastructure.Data;
using Jurupema.Api.Infrastructure.Data.Repositories;
using Jurupema.Api.Infrastructure.Messaging;
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
        // Infra
        builder.Services.AddDbContext<JurupemaDbContext>(options =>
        {
            if (builder.Configuration.GetValue<bool>("Database:UseInMemory"))
            {
                var name = builder.Configuration["Database:InMemoryDatabaseName"] ?? "JurupemaInMemory";
                options.UseInMemoryDatabase(name);
            }
            else
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("database"));
            }
        });
        builder.Services.Configure<StorageConfiguration>(builder.Configuration.GetSection(StorageConfiguration.Position));
        builder.Services.Configure<ServiceBusConfiguration>(builder.Configuration.GetSection(ServiceBusConfiguration.Position));

        builder.Services.AddSingleton<IOrderServiceBusTopics, OrderServiceBusTopics>();
        builder.Services.AddSingleton<IStorageClient, BlobStorageClient>();
        
        builder.RegisterServiceBus();
        
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        builder.AddServiceDefaults();

        // Application
        builder.Services.AddScoped<ProductService>();
        builder.Services.AddScoped<OrderService>();
        builder.Services.AddScoped<IOrderCreatedMessageHandler, OrderCreatedMessageHandler>();
        builder.Services.AddScoped<ProductImageService>();
        builder.Services.AddValidatorsFromAssemblyContaining<CreateProductParameterValidator>();

        return builder;
    }

    public static async Task RunMigrationsAsync(this WebApplication builder)
    {
        using var scope = builder.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<JurupemaDbContext>();
        if (dbContext.Database.IsRelational())
            await dbContext.Database.MigrateAsync();
    }
}
