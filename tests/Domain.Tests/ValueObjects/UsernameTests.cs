using System;
using FluentAssertions;
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
        username.Value.Should().Be(validUsername.ToLowerInvariant());
    }

    [Fact]
    public void Create_WithValidUsernameUppercase_ShouldConvertToLowercase()
    {
        // Arrange
        var validUsername = "USER123";

        // Act
        var username = new Username(validUsername);

        // Assert
        username.Value.Should().Be("user123");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData(null)]
    public void Create_WithEmptyOrWhitespace_ShouldThrowArgumentException(string invalidUsername)
    {
        // Act & Assert
        var action = () => new Username(invalidUsername);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Username cannot be empty*");
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
        var action = () => new Username(invalidUsername);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Username must be 3-20 characters, alphanumeric and underscore only*");
    }

    [Fact]
    public void Create_WithUsernameHavingWhitespace_ShouldTrimWhitespace()
    {
        // Arrange
        var usernameWithWhitespace = "  username  ";

        // Act
        var username = new Username(usernameWithWhitespace);

        // Assert
        username.Value.Should().Be("username");
    }

    [Fact]
    public void ImplicitConversion_ToString_ShouldReturnValue()
    {
        // Arrange
        var username = new Username("testuser");

        // Act
        string result = username;

        // Assert
        result.Should().Be("testuser");
    }
}
