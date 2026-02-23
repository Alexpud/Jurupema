using Jurupema.Api.Apis;
using Jurupema.Api.Application.Storage;
using Jurupema.Api.Infrastructure;
using Jurupema.Api.Infrastructure.Storage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<IStorageClient, BlobStorageClient>();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "Jurupema API",
        Version = "v1"
    });
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

app.MapProductOrderEndpoints();
app.MapProductImageEndpoints();
app.UseAntiforgery();

app.Run();