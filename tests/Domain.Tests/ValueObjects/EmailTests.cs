using System;
using UserService.Domain.ValueObjects;
using Xunit;

namespace UserService.Tests.Domain.ValueObjects;

public class EmailTests
{
    [Fact]
    public void Create_WithValidEmail_ShouldSucceed()
    {
        // Arrange
        var validEmail = "user@example.com";

        // Act
        var email = new Email(validEmail);

        // Assert
        Assert.Equal("user@example.com", email.Value);
    }

    [Fact]
    public void Create_WithValidEmailUppercase_ShouldConvertToLowercase()
    {
        // Arrange
        var validEmail = "USER@EXAMPLE.COM";

        // Act
        var email = new Email(validEmail);

        // Assert
        Assert.Equal("user@example.com", email.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData(null)]
    public void Create_WithEmptyOrWhitespace_ShouldThrowArgumentException(string invalidEmail)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Email(invalidEmail));
        Assert.StartsWith("Email cannot be empty", exception.Message);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("invalid@")]
    [InlineData("@invalid")]
    [InlineData("invalid@.com")]
    [InlineData("invalid.email")]
    public void Create_WithInvalidFormat_ShouldThrowArgumentException(string invalidEmail)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Email(invalidEmail));
        Assert.StartsWith("Invalid email format", exception.Message);
    }

    [Fact]
    public void Create_WithEmailHavingWhitespace_ShouldTrimWhitespace()
    {
        // Arrange
        var emailWithWhitespace = "  user@example.com  ";

        // Act
        var email = new Email(emailWithWhitespace);

        // Assert
        Assert.Equal("user@example.com", email.Value);
    }

    [Fact]
    public void ImplicitConversion_ToString_ShouldReturnValue()
    {
        // Arrange
        var email = new Email("user@example.com");

        // Act
        string result = email;

        // Assert
        Assert.Equal("user@example.com", result);
    }

    [Fact]
    public void ExplicitConversion_FromString_ShouldCreateEmail()
    {
        // Arrange
        var emailString = "user@example.com";

        // Act
        var email = (Email)emailString;

        // Assert
        Assert.Equal("user@example.com", email.Value);
    }
}
