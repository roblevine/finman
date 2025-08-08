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

}
