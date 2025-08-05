using System;
using FluentAssertions;
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
        email.Value.Should().Be("user@example.com");
    }

    [Fact]
    public void Create_WithValidEmailUppercase_ShouldConvertToLowercase()
    {
        // Arrange
        var validEmail = "USER@EXAMPLE.COM";

        // Act
        var email = new Email(validEmail);

        // Assert
        email.Value.Should().Be("user@example.com");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData(null)]
    public void Create_WithEmptyOrWhitespace_ShouldThrowArgumentException(string invalidEmail)
    {
        // Act & Assert
        var action = () => new Email(invalidEmail);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Email cannot be empty*");
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
        var action = () => new Email(invalidEmail);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Invalid email format*");
    }

    [Fact]
    public void Create_WithEmailHavingWhitespace_ShouldTrimWhitespace()
    {
        // Arrange
        var emailWithWhitespace = "  user@example.com  ";

        // Act
        var email = new Email(emailWithWhitespace);

        // Assert
        email.Value.Should().Be("user@example.com");
    }

    [Fact]
    public void ImplicitConversion_ToString_ShouldReturnValue()
    {
        // Arrange
        var email = new Email("user@example.com");

        // Act
        string result = email;

        // Assert
        result.Should().Be("user@example.com");
    }

    [Fact]
    public void ExplicitConversion_FromString_ShouldCreateEmail()
    {
        // Arrange
        var emailString = "user@example.com";

        // Act
        var email = (Email)emailString;

        // Assert
        email.Value.Should().Be("user@example.com");
    }
}
