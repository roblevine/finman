using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net;
using System.Text.Json;
using Xunit;

namespace UserService.Tests.Infrastructure.Tests;

public class HealthCheckTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public HealthCheckTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task HealthCheck_Endpoint_ReturnsHealthy()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal("Healthy", content);
    }

    [Fact]
    public async Task HealthCheck_Service_ReturnsHealthyResult()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var healthCheckService = scope.ServiceProvider.GetRequiredService<HealthCheckService>();

        // Act
        var result = await healthCheckService.CheckHealthAsync();

        // Assert
        Assert.Equal(HealthStatus.Healthy, result.Status);
        Assert.Contains("self", result.Entries.Keys);
        Assert.Equal(HealthStatus.Healthy, result.Entries["self"].Status);
        Assert.Equal("Service is running", result.Entries["self"].Description);
    }

    [Fact]
    public void HealthCheck_Registration_IsConfigured()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var healthCheckService = scope.ServiceProvider.GetService<HealthCheckService>();

        // Assert
        Assert.NotNull(healthCheckService);
    }
}