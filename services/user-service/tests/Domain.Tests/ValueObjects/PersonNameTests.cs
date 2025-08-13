using System;
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
        Assert.Equal(validName, name.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData(null)]
    public void Create_WithEmptyOrWhitespace_ShouldThrowArgumentException(string invalidName)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new PersonName(invalidName));
        Assert.StartsWith("Name cannot be empty", exception.Message);
    }

    [Fact]
    public void Create_WithNameTooLong_ShouldThrowArgumentException()
    {
        // Arrange
        var tooLongName = new string('a', 51);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new PersonName(tooLongName));
        Assert.StartsWith("Name cannot exceed 50 characters", exception.Message);
    }

    [Fact]
    public void Create_WithNameHavingWhitespace_ShouldTrimWhitespace()
    {
        // Arrange
        var nameWithWhitespace = "  John  ";

        // Act
        var name = new PersonName(nameWithWhitespace);

        // Assert
        Assert.Equal("John", name.Value);
    }

    [Fact]
    public void Create_WithMaxLengthName_ShouldSucceed()
    {
        // Arrange
        var maxLengthName = new string('a', 50);

        // Act
        var name = new PersonName(maxLengthName);

        // Assert
        Assert.Equal(maxLengthName, name.Value);
    }
}
