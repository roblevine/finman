using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;
using Xunit;

namespace UserService.Tests.Infrastructure.Tests;

public class ApiIntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiIntegrationTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    [Fact]
    public async Task HelloEndpoint_ReturnsSuccessAndCorrectContentType()
    {
        // Act
        var response = await _client.GetAsync("/api/hello");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task HelloEndpoint_ReturnsExpectedResponse()
    {
        // Act
        var response = await _client.GetAsync("/api/hello");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var helloResponse = JsonSerializer.Deserialize<HelloResponse>(content, _jsonOptions);
        Assert.NotNull(helloResponse);
        Assert.Equal("Hello from Finman User Service!", helloResponse.Message);
        Assert.Equal("1.0.0", helloResponse.Version);
        Assert.True(helloResponse.Timestamp <= DateTime.UtcNow);
        Assert.True(helloResponse.Timestamp > DateTime.UtcNow.AddMinutes(-1));
    }

    [Theory]
    [InlineData("John")]
    [InlineData("Alice")]
    [InlineData("Test User")]
    public async Task HelloPersonalizedEndpoint_WithValidName_ReturnsExpectedResponse(string name)
    {
        // Act
        var response = await _client.GetAsync($"/api/hello/{Uri.EscapeDataString(name)}");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var helloResponse = JsonSerializer.Deserialize<HelloResponse>(content, _jsonOptions);
        Assert.NotNull(helloResponse);
        Assert.Equal($"Hello {name}, welcome to Finman User Service!", helloResponse.Message);
        Assert.Equal("1.0.0", helloResponse.Version);
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("   ")]
    public async Task HelloPersonalizedEndpoint_WithInvalidName_ReturnsBadRequest(string name)
    {
        // Act
        var response = await _client.GetAsync($"/api/hello/{Uri.EscapeDataString(name)}");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        // The API returns a structured validation response from model validation
        Assert.Contains("validation errors occurred", content);
    }

    [Fact]
    public async Task HelloPersonalizedEndpoint_WithEmptyName_ReturnsBasicHello()
    {
        // Empty string gets routed to the basic hello endpoint due to URL routing
        // Act
        var response = await _client.GetAsync("/api/hello/");
        var content = await response.Content.ReadAsStringAsync();

        // Assert  
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var helloResponse = JsonSerializer.Deserialize<HelloResponse>(content, _jsonOptions);
        Assert.NotNull(helloResponse);
        Assert.Equal("Hello from Finman User Service!", helloResponse.Message);
    }

    [Fact]
    public async Task SwaggerEndpoint_IsAccessible()
    {
        // Act
        var response = await _client.GetAsync("/swagger/v1/swagger.json");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Finman User Service API", content);
        Assert.Contains("/api/hello", content);
    }

    public record HelloResponse
    {
        public string Message { get; init; } = string.Empty;
        public DateTime Timestamp { get; init; }
        public string Version { get; init; } = string.Empty;
    }
}