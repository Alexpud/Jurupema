using Microsoft.EntityFrameworkCore;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var password = builder.AddParameter("db-password").WithDescription("The password for the SQL Server database.");

var database = builder.AddSqlServer("jurupema-sqlServer", password: password)
    .WithDataVolume()
    .WithEndpoint(port: 1433, targetPort: 1433, name: "ssms", isProxied: false)
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("jurupema-db");

builder.AddProject<Jurupema_Api>("jurupema-api")
    .WithReference(database)
    .WaitFor(database);


builder.Build().Run();
