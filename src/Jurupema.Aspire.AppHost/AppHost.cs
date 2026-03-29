using Aspire.Hosting.Azure;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var password = builder.AddParameter("db-password").WithDescription("The password for the SQL Server database.");

var database = builder.AddSqlServer("jurupema-sqlServer", password: password)
    .WithDataVolume()
    .WithEndpoint(port: 1433, targetPort: 1433, name: "ssms", isProxied: false)
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("database");

var serviceBus = builder.AddAzureServiceBus("messaging")
    .RunAsEmulator();

var orderCreatedTopic = serviceBus.AddServiceBusTopic("sbt-jurupema-order-created");
orderCreatedTopic.AddServiceBusSubscription("order-created-consumer", "sbts-jurupema-order-created");

builder.AddProject<Jurupema_Api>("jurupema-api")
    .WithReference(database)
    .WithReference(serviceBus)
    .WaitFor(database)
    .WaitFor(serviceBus)
    .WithEnvironment("ServiceBus__Enabled", "true");


builder.Build().Run();
