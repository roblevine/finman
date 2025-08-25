using System.Net;
using System.Net.Http.Json;
using UserService.Application.DTOs;
using Xunit;

namespace UserService.Tests.Infrastructure.Tests;

/// <summary>
/// Integration tests demonstrating switchable test infrastructure.
/// Run with:
/// - Default: dotnet test (uses InMemory for fast feedback)  
/// - PostgreSQL: USE_TESTCONTAINERS=true dotnet test (uses real PostgreSQL)
/// </summary>
public class SwitchableDatabaseIntegrationTests : SwitchableWebApplicationFactoryTests
{
    public SwitchableDatabaseIntegrationTests(PostgreSqlFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task HealthCheck_ShouldReturn_Healthy()
    {
        // Arrange & Act
        var response = await Client.GetAsync("/health");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Healthy", content);
    }

    [Fact]
    public async Task RegisterUser_ValidRequest_ShouldSucceed()
    {
        // Arrange
        var request = new RegisterRequest
        {
            FirstName = "Test",
            LastName = "User", 
            Email = $"test.user.{Guid.NewGuid()}@example.com", // Unique email
            Username = $"testuser{Guid.NewGuid():N}"[..15], // Ensure username is within limits
            Password = "SecurePassword123!"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var registerResponse = await response.Content.ReadFromJsonAsync<RegisterResponse>();
        Assert.NotNull(registerResponse);
        Assert.NotEqual(Guid.Empty, registerResponse.Id);
        Assert.Equal(request.Email, registerResponse.Email);
        Assert.Equal(request.Username, registerResponse.Username);
        
        // Verify backend type for transparency
        var expectedBackend = IsUsingPostgreSQL ? "PostgreSQL" : "InMemory";
        // Note: This is just for test clarity - the API behavior should be identical
    }

    [Fact]
    public async Task RegisterUser_DuplicateEmail_ShouldFail()
    {
        // Arrange - Register first user
        var email = $"duplicate.test.{Guid.NewGuid()}@example.com";
        var firstUserRequest = new RegisterRequest
        {
            FirstName = "First",
            LastName = "User",
            Email = email,
            Username = $"firstuser{Guid.NewGuid():N}"[..15],
            Password = "SecurePassword123!"
        };

        var firstResponse = await Client.PostAsJsonAsync("/api/auth/register", firstUserRequest);
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        // Act - Try to register second user with same email
        var secondUserRequest = new RegisterRequest
        {
            FirstName = "Second", 
            LastName = "User",
            Email = email, // Same email
            Username = $"seconduser{Guid.NewGuid():N}"[..15],
            Password = "SecurePassword123!"
        };

        var secondResponse = await Client.PostAsJsonAsync("/api/auth/register", secondUserRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
        
        var responseContent = await secondResponse.Content.ReadAsStringAsync();
        Assert.Contains("email", responseContent.ToLower());
    }

}