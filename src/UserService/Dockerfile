# Use the official .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY UserService.csproj .
COPY Tests/Domain.Tests/Domain.Tests.csproj Tests/Domain.Tests/
COPY Tests/Application.Tests/Application.Tests.csproj Tests/Application.Tests/
COPY Tests/Infrastructure.Tests/Infrastructure.Tests.csproj Tests/Infrastructure.Tests/

# Restore dependencies
RUN dotnet restore UserService.csproj

# Copy all source code
COPY . .

# Run tests
RUN dotnet test --no-restore --verbosity normal

# Build the application
RUN dotnet build UserService.csproj -c Release --no-restore

# Publish the application
RUN dotnet publish UserService.csproj -c Release -o /app/publish --no-restore

# Use the official .NET 8 runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create a non-root user for security
RUN useradd -r -u 1001 appuser

# Copy the published application
COPY --from=build /app/publish .

# Set ownership and permissions
RUN chown -R appuser:appuser /app
USER appuser

# Expose the port the app runs on
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Run the application
ENTRYPOINT ["dotnet", "UserService.dll"]