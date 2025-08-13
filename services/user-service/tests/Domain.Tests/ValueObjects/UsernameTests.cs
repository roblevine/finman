using System;
using UserService.Domain.ValueObjects;
using Xunit;

namespace UserService.Tests.Domain.ValueObjects;

public class UsernameTests
{
    [Theory]
    [InlineData("user")]
    [InlineData("user123")]
    [InlineData("user_name")]
    [InlineData("User123")]
    [InlineData("test_user_123")]
    public void Create_WithValidUsername_ShouldSucceed(string validUsername)
    {
        // Act
        var username = new Username(validUsername);

        // Assert
        Assert.Equal(validUsername.ToLowerInvariant(), username.Value);
    }

    [Fact]
    public void Create_WithValidUsernameUppercase_ShouldConvertToLowercase()
    {
        // Arrange
        var validUsername = "USER123";

        // Act
        var username = new Username(validUsername);

        // Assert
        Assert.Equal("user123", username.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData(null)]
    public void Create_WithEmptyOrWhitespace_ShouldThrowArgumentException(string invalidUsername)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Username(invalidUsername));
        Assert.StartsWith("Username cannot be empty", exception.Message);
    }

    [Theory]
    [InlineData("ab")] // Too short
    [InlineData("a")] // Too short
    [InlineData("this_username_is_way_too_long")] // Too long
    [InlineData("user@name")] // Invalid character
    [InlineData("user name")] // Space
    [InlineData("user-name")] // Hyphen
    [InlineData("user.name")] // Period
    public void Create_WithInvalidFormat_ShouldThrowArgumentException(string invalidUsername)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Username(invalidUsername));
        Assert.StartsWith("Username must be 3-20 characters, alphanumeric and underscore only", exception.Message);
    }

    [Fact]
    public void Create_WithUsernameHavingWhitespace_ShouldTrimWhitespace()
    {
        // Arrange
        var usernameWithWhitespace = "  username  ";

        // Act
        var username = new Username(usernameWithWhitespace);

        // Assert
        Assert.Equal("username", username.Value);
    }

    [Fact]
    public void ImplicitConversion_ToString_ShouldReturnValue()
    {
        // Arrange
        var username = new Username("testuser");

        // Act
        string result = username;

        // Assert
        Assert.Equal("testuser", result);
    }
}
