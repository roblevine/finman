using System;
using FluentAssertions;
using UserService.Domain.Entities;
using UserService.Domain.ValueObjects;
using UserService.Domain.Exceptions;
using Xunit;

namespace UserService.Tests.Domain.Entities;

public class UserTests
{
    private readonly Email _validEmail = new("user@example.com");
    private readonly Username _validUsername = new("testuser");
    private readonly PersonName _validFirstName = new("John");
    private readonly PersonName _validLastName = new("Doe");
    private const string ValidPasswordHash = "hashedpassword123";

    [Fact]
    public void Create_WithValidParameters_ShouldCreateUser()
    {
        // Act
        var user = User.Create(_validEmail, _validUsername, _validFirstName, _validLastName, ValidPasswordHash);

        // Assert
        user.Should().NotBeNull();
        user.Id.Should().NotBeEmpty();
        user.Email.Should().Be(_validEmail);
        user.Username.Should().Be(_validUsername);
        user.FirstName.Should().Be(_validFirstName);
        user.LastName.Should().Be(_validLastName);
        user.PasswordHash.Should().Be(ValidPasswordHash);
        user.FullName.Should().Be("John Doe");
        user.IsActive.Should().BeTrue();
        user.IsDeleted.Should().BeFalse();
        user.DeletedAt.Should().BeNull();
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidPasswordHash_ShouldThrowUserDomainException(string invalidPasswordHash)
    {
        // Act & Assert
        var action = () => User.Create(_validEmail, _validUsername, _validFirstName, _validLastName, invalidPasswordHash);
        action.Should().Throw<UserDomainException>()
            .WithMessage("Password hash cannot be empty");
    }

    [Fact]
    public void UpdateProfile_WithValidNames_ShouldUpdateNamesAndTimestamp()
    {
        // Arrange
        var user = User.Create(_validEmail, _validUsername, _validFirstName, _validLastName, ValidPasswordHash);
        var originalUpdatedAt = user.UpdatedAt;
        var newFirstName = new PersonName("Jane");
        var newLastName = new PersonName("Smith");

        // Act
        user.UpdateProfile(newFirstName, newLastName);

        // Assert
        user.FirstName.Should().Be(newFirstName);
        user.LastName.Should().Be(newLastName);
        user.FullName.Should().Be("Jane Smith");
        user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void UpdatePassword_WithValidHash_ShouldUpdatePasswordAndTimestamp()
    {
        // Arrange
        var user = User.Create(_validEmail, _validUsername, _validFirstName, _validLastName, ValidPasswordHash);
        var originalUpdatedAt = user.UpdatedAt;
        var newPasswordHash = "newhash456";

        // Act
        user.UpdatePassword(newPasswordHash);

        // Assert
        user.PasswordHash.Should().Be(newPasswordHash);
        user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void UpdatePassword_WithInvalidHash_ShouldThrowUserDomainException(string invalidHash)
    {
        // Arrange
        var user = User.Create(_validEmail, _validUsername, _validFirstName, _validLastName, ValidPasswordHash);

        // Act & Assert
        var action = () => user.UpdatePassword(invalidHash);
        action.Should().Throw<UserDomainException>()
            .WithMessage("Password hash cannot be empty");
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalseAndUpdateTimestamp()
    {
        // Arrange
        var user = User.Create(_validEmail, _validUsername, _validFirstName, _validLastName, ValidPasswordHash);
        var originalUpdatedAt = user.UpdatedAt;

        // Act
        user.Deactivate();

        // Assert
        user.IsActive.Should().BeFalse();
        user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void Activate_ShouldSetIsActiveToTrueAndUpdateTimestamp()
    {
        // Arrange
        var user = User.Create(_validEmail, _validUsername, _validFirstName, _validLastName, ValidPasswordHash);
        user.Deactivate();
        var originalUpdatedAt = user.UpdatedAt;

        // Act
        user.Activate();

        // Assert
        user.IsActive.Should().BeTrue();
        user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void SoftDelete_ShouldSetDeletedFlagsAndUpdateTimestamp()
    {
        // Arrange
        var user = User.Create(_validEmail, _validUsername, _validFirstName, _validLastName, ValidPasswordHash);
        var originalUpdatedAt = user.UpdatedAt;

        // Act
        user.SoftDelete();

        // Assert
        user.IsDeleted.Should().BeTrue();
        user.DeletedAt.Should().NotBeNull();
        user.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void Restore_ShouldClearDeletedFlagsAndUpdateTimestamp()
    {
        // Arrange
        var user = User.Create(_validEmail, _validUsername, _validFirstName, _validLastName, ValidPasswordHash);
        user.SoftDelete();
        var originalUpdatedAt = user.UpdatedAt;

        // Act
        user.Restore();

        // Assert
        user.IsDeleted.Should().BeFalse();
        user.DeletedAt.Should().BeNull();
        user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }
}
