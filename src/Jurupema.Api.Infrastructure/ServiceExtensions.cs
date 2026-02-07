using Jurupema.Api.Infrastructure.Storage;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jurupema.Api.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddSingleton<IStorageClient, BlobStorageClient>();
        return services;
    }
}
