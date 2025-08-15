using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using UserService.Application.DTOs;
using UserService.Application.Ports;
using UserService.Domain.Entities;
using UserService.Domain.ValueObjects;
using Xunit;

namespace UserService.Tests.Infrastructure.Tests;

public class AuthControllerTests : IClassFixture<AuthTestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly AuthTestWebApplicationFactory _factory;

    public AuthControllerTests(AuthTestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidRequest_ShouldReturn201Created()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Username = "testuser",
            FirstName = "Test",
            LastName = "User",
            Password = "SecurePass123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var registerResponse = await response.Content.ReadFromJsonAsync<RegisterResponse>();
        Assert.NotNull(registerResponse);
        Assert.NotEqual(Guid.Empty, registerResponse.Id);
        Assert.Equal("test@example.com", registerResponse.Email);
        Assert.Equal("testuser", registerResponse.Username);
        Assert.Equal("Test User", registerResponse.FullName);
        Assert.True(registerResponse.CreatedAt <= DateTime.UtcNow);
        Assert.True(registerResponse.CreatedAt > DateTime.UtcNow.AddSeconds(-5));

        // Check Location header
        Assert.NotNull(response.Headers.Location);
        Assert.Contains($"/api/users/{registerResponse.Id}", response.Headers.Location.ToString());

        // Verify Content-Type
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ShouldReturn409Conflict()
    {
        // Arrange - create first user
        var firstRequest = new RegisterRequest
        {
            Email = "duplicate@example.com",
            Username = "firstuser",
            FirstName = "First",
            LastName = "User",
            Password = "SecurePass123!"
        };
        await _client.PostAsJsonAsync("/api/auth/register", firstRequest);

        // Arrange - attempt to create user with same email
        var duplicateRequest = new RegisterRequest
        {
            Email = "duplicate@example.com", // Same email
            Username = "seconduser", // Different username
            FirstName = "Second",
            LastName = "User",
            Password = "SecurePass123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", duplicateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problemDetails);
        Assert.Equal("User registration failed", problemDetails.Title);
        Assert.Contains("email", problemDetails.Detail?.ToLower());
        Assert.Equal(409, problemDetails.Status);

        // Verify Content-Type for error responses
        Assert.Equal("application/problem+json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task Register_WithDuplicateUsername_ShouldReturn409Conflict()
    {
        // Arrange - create first user
        var firstRequest = new RegisterRequest
        {
            Email = "first@example.com",
            Username = "duplicateuser",
            FirstName = "First",
            LastName = "User",
            Password = "SecurePass123!"
        };
        await _client.PostAsJsonAsync("/api/auth/register", firstRequest);

        // Arrange - attempt to create user with same username
        var duplicateRequest = new RegisterRequest
        {
            Email = "second@example.com", // Different email
            Username = "duplicateuser", // Same username
            FirstName = "Second",
            LastName = "User",
            Password = "SecurePass123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", duplicateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problemDetails);
        Assert.Equal("User registration failed", problemDetails.Title);
        Assert.Contains("username", problemDetails.Detail?.ToLower());
        Assert.Equal(409, problemDetails.Status);
    }

    [Theory]
    [InlineData("", "testuser", "Test", "User", "SecurePass123!", "Email is required")]
    [InlineData("invalid-email", "testuser", "Test", "User", "SecurePass123!", "Invalid email format")]
    [InlineData("test@example.com", "", "Test", "User", "SecurePass123!", "Username is required")]
    [InlineData("test@example.com", "ab", "Test", "User", "SecurePass123!", "Username must be between 3 and 20 characters")]
    [InlineData("test@example.com", "user-with-dash", "Test", "User", "SecurePass123!", "Username can only contain letters, numbers, and underscores")]
    [InlineData("test@example.com", "testuser", "", "User", "SecurePass123!", "First name is required")]
    [InlineData("test@example.com", "testuser", "Test", "", "SecurePass123!", "Last name is required")]
    [InlineData("test@example.com", "testuser", "Test", "User", "", "Password is required")]
    [InlineData("test@example.com", "testuser", "Test", "User", "short", "Password must be at least 8 characters long")]
    public async Task Register_WithInvalidRequest_ShouldReturn400BadRequest(
        string email, string username, string firstName, string lastName, string password, string expectedError)
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = email,
            Username = username,
            FirstName = firstName,
            LastName = lastName,
            Password = password
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var validationProblem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(validationProblem);
        Assert.Equal("One or more validation errors occurred.", validationProblem.Title);
        Assert.Equal(400, validationProblem.Status);

        // Check that validation errors contain expected message
        var allErrors = validationProblem.Errors.SelectMany(kvp => kvp.Value).ToArray();
        Assert.Contains(allErrors, error => error.Contains(expectedError));

        // Verify Content-Type for validation errors
        Assert.Equal("application/problem+json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task Register_WithMalformedJson_ShouldReturn400BadRequest()
    {
        // Arrange
        var malformedJson = "{ invalid json }";
        var content = new StringContent(malformedJson, System.Text.Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/auth/register", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithEmptyBody_ShouldReturn400BadRequest()
    {
        // Arrange
        var content = new StringContent("", System.Text.Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/auth/register", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithNullRequest_ShouldReturn400BadRequest()
    {
        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", (RegisterRequest?)null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_ResponseFormat_ShouldBeCamelCase()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "format@example.com",
            Username = "formatuser",
            FirstName = "Format",
            LastName = "User",
            Password = "SecurePass123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var jsonResponse = await response.Content.ReadAsStringAsync();
        
        // Verify camelCase field names in JSON response
        Assert.Contains("\"id\":", jsonResponse);
        Assert.Contains("\"email\":", jsonResponse);
        Assert.Contains("\"username\":", jsonResponse);
        Assert.Contains("\"fullName\":", jsonResponse);
        Assert.Contains("\"createdAt\":", jsonResponse);

        // Verify no PascalCase field names
        Assert.DoesNotContain("\"Id\":", jsonResponse);
        Assert.DoesNotContain("\"Email\":", jsonResponse);
        Assert.DoesNotContain("\"Username\":", jsonResponse);
        Assert.DoesNotContain("\"FullName\":", jsonResponse);
        Assert.DoesNotContain("\"CreatedAt\":", jsonResponse);
    }

    [Fact]
    public async Task Register_ShouldPersistUserCorrectly()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "persist@example.com",
            Username = "persistuser",
            FirstName = "Persist",
            LastName = "User",
            Password = "SecurePass123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);
        var registerResponse = await response.Content.ReadFromJsonAsync<RegisterResponse>();

        // Assert - verify user was actually persisted
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        
        // Check email uniqueness (should return true since user exists)
        var emailExists = await repository.ExistsByEmailAsync(new Email("persist@example.com"));
        Assert.True(emailExists);

        // Check username uniqueness (should return true since user exists)
        var usernameExists = await repository.ExistsByUsernameAsync(new Username("persistuser"));
        Assert.True(usernameExists);
    }
}
