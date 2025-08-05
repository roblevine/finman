using System;
using FluentAssertions;
using UserService.Domain.ValueObjects;
using Xunit;

namespace UserService.Tests.Domain.ValueObjects;

public class PersonNameTests
{
    [Theory]
    [InlineData("John")]
    [InlineData("Mary-Jane")]
    [InlineData("José")]
    [InlineData("O'Connor")]
    public void Create_WithValidName_ShouldSucceed(string validName)
    {
        // Act
        var name = new PersonName(validName);

        // Assert
        name.Value.Should().Be(validName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData(null)]
    public void Create_WithEmptyOrWhitespace_ShouldThrowArgumentException(string invalidName)
    {
        // Act & Assert
        var action = () => new PersonName(invalidName);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Name cannot be empty*");
    }

    [Fact]
    public void Create_WithNameTooLong_ShouldThrowArgumentException()
    {
        // Arrange
        var tooLongName = new string('a', 51);

        // Act & Assert
        var action = () => new PersonName(tooLongName);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Name cannot exceed 50 characters*");
    }

    [Fact]
    public void Create_WithNameHavingWhitespace_ShouldTrimWhitespace()
    {
        // Arrange
        var nameWithWhitespace = "  John  ";

        // Act
        var name = new PersonName(nameWithWhitespace);

        // Assert
        name.Value.Should().Be("John");
    }

    [Fact]
    public void Create_WithMaxLengthName_ShouldSucceed()
    {
        // Arrange
        var maxLengthName = new string('a', 50);

        // Act
        var name = new PersonName(maxLengthName);

        // Assert
        name.Value.Should().Be(maxLengthName);
    }
}
