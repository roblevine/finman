using System;
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
        Assert.NotNull(user);
        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal(_validEmail, user.Email);
        Assert.Equal(_validUsername, user.Username);
        Assert.Equal(_validFirstName, user.FirstName);
        Assert.Equal(_validLastName, user.LastName);
        Assert.Equal(ValidPasswordHash, user.PasswordHash);
        Assert.Equal("John Doe", user.FullName);
        Assert.True(user.IsActive);
        Assert.False(user.IsDeleted);
        Assert.Null(user.DeletedAt);
        Assert.True(Math.Abs((DateTime.UtcNow - user.CreatedAt).TotalSeconds) < 1);
        Assert.True(Math.Abs((DateTime.UtcNow - user.UpdatedAt).TotalSeconds) < 1);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidPasswordHash_ShouldThrowUserDomainException(string invalidPasswordHash)
    {
        // Act & Assert
        var exception = Assert.Throws<UserDomainException>(() => 
            User.Create(_validEmail, _validUsername, _validFirstName, _validLastName, invalidPasswordHash));
        Assert.Equal("Password hash cannot be empty", exception.Message);
    }

}
