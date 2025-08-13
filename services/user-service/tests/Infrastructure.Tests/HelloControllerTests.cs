using Microsoft.AspNetCore.Mvc;
using UserService.Infrastructure.Web.Controllers;
using Xunit;

namespace UserService.Tests.Infrastructure.Tests;

public class HelloControllerTests
{
    private readonly HelloController _controller;

    public HelloControllerTests()
    {
        _controller = new HelloController();
    }

    [Fact]
    public void Get_ReturnsOkResult_WithHelloResponse()
    {
        // Act
        var result = _controller.Get();

        // Assert
        var actionResult = Assert.IsType<ActionResult<HelloResponse>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<HelloResponse>(okResult.Value);
        
        Assert.Equal("Hello from Finman User Service!", response.Message);
        Assert.Equal("1.0.0", response.Version);
        Assert.True(response.Timestamp <= DateTime.UtcNow);
        Assert.True(response.Timestamp > DateTime.UtcNow.AddSeconds(-1));
    }

    [Theory]
    [InlineData("John")]
    [InlineData("Alice")]
    [InlineData("Bob")]
    public void GetPersonalized_WithValidName_ReturnsOkResult_WithPersonalizedMessage(string name)
    {
        // Act
        var result = _controller.GetPersonalized(name);

        // Assert
        var actionResult = Assert.IsType<ActionResult<HelloResponse>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<HelloResponse>(okResult.Value);
        
        Assert.Equal($"Hello {name}, welcome to Finman User Service!", response.Message);
        Assert.Equal("1.0.0", response.Version);
        Assert.True(response.Timestamp <= DateTime.UtcNow);
        Assert.True(response.Timestamp > DateTime.UtcNow.AddSeconds(-1));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData(null)]
    public void GetPersonalized_WithInvalidName_ReturnsBadRequest(string? name)
    {
        // Act
        var result = _controller.GetPersonalized(name!);

        // Assert
        var badRequestResult = Assert.IsType<ActionResult<HelloResponse>>(result);
        var badRequest = Assert.IsType<BadRequestObjectResult>(badRequestResult.Result);
        Assert.Equal("Name cannot be empty", badRequest.Value);
    }

    [Fact]
    public void HelloResponse_Properties_CanBeSet()
    {
        // Arrange
        var timestamp = DateTime.UtcNow;
        
        // Act
        var response = new HelloResponse
        {
            Message = "Test message",
            Timestamp = timestamp,
            Version = "2.0.0"
        };

        // Assert
        Assert.Equal("Test message", response.Message);
        Assert.Equal(timestamp, response.Timestamp);
        Assert.Equal("2.0.0", response.Version);
    }

    [Fact]
    public void HelloResponse_DefaultValues_AreCorrect()
    {
        // Act
        var response = new HelloResponse();

        // Assert
        Assert.Equal(string.Empty, response.Message);
        Assert.Equal(default(DateTime), response.Timestamp);
        Assert.Equal(string.Empty, response.Version);
    }
}