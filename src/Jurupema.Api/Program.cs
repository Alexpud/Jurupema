using System.Text.Json.Serialization;
using Jurupema.Api.Apis;
using Jurupema.Api.Application.Storage;
using Jurupema.Api.Infrastructure;
using Jurupema.Api.Infrastructure.Storage;
using Jurupema.Api.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<IStorageClient, BlobStorageClient>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "Jurupema API",
        Version = "v1"
    });
    options.SchemaFilter<EnumAsStringSchemaFilter>();
});

builder.Services.AddAntiforgery();
builder.RegisterServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Jurupema API v1");
    });
}

await app.RunMigrationsAsync();

app.UseHttpsRedirection();

app.MapProductEndpoints();
app.MapOrderEndpoints();
app.MapProductImageEndpoints();
app.UseAntiforgery();

app.Run();