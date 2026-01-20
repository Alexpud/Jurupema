# Use the official .NET runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Copy the published application files from the build context
# Assumes the files are in a 'publish' folder relative to the Dockerfile
COPY ./publish .

# Set the entry point
ENTRYPOINT ["dotnet", "Jurupema.Api.dll"]