using UserService.Domain.ValueObjects;
using UserService.Domain.Entities;
using UserService.Domain.Exceptions;
using UserService.Tests.Application.Tests.Mocks;
using Xunit;

namespace UserService.Tests.Application.Tests.UseCases;

public class RegisterUserTests
{
    private readonly MockUserRepository _mockRepo;
    private readonly MockPasswordHasher _mockHasher;

    public RegisterUserTests()
    {
        _mockRepo = new MockUserRepository();
        _mockHasher = new MockPasswordHasher();
    }

    [Fact]
    public async Task RegisterUser_WithValidData_ShouldSucceed()
    {
        // Arrange
        var email = new Email("john.doe@example.com");
        var username = new Username("johndoe");
        var firstName = new PersonName("John");
        var lastName = new PersonName("Doe");
        var plainPassword = "SecurePass123!";

        // Act
        var hashedPassword = _mockHasher.HashPassword(plainPassword);
        var user = User.Create(email, username, firstName, lastName, hashedPassword);
        var addedUser = await _mockRepo.AddAsync(user);

        // Assert
        Assert.NotNull(addedUser);
        Assert.Equal(email, addedUser.Email);
        Assert.Equal(username, addedUser.Username);
        Assert.Equal(firstName, addedUser.FirstName);
        Assert.Equal(lastName, addedUser.LastName);
        Assert.Equal("hash:SecurePass123!", addedUser.PasswordHash);
        
        // Verify uniqueness checks would have passed
        Assert.False(await _mockRepo.IsEmailUniqueAsync(email)); // now taken
        Assert.False(await _mockRepo.IsUsernameUniqueAsync(username)); // now taken
    }

    [Fact]
    public async Task RegisterUser_WithExistingEmail_ShouldFailUniquenessCheck()
    {
        // Arrange
        var existingEmail = new Email("existing@example.com");
        var existingUser = User.Create(
            existingEmail,
            new Username("existing"),
            new PersonName("Existing"),
            new PersonName("User"),
            _mockHasher.HashPassword("password"));
        _mockRepo.SeedUser(existingUser);

        // Act & Assert
        var isUnique = await _mockRepo.IsEmailUniqueAsync(existingEmail);
        Assert.False(isUnique);
    }

    [Fact]
    public async Task RegisterUser_WithExistingUsername_ShouldFailUniquenessCheck()
    {
        // Arrange
        var existingUsername = new Username("existinguser");
        var existingUser = User.Create(
            new Email("existing@example.com"),
            existingUsername,
            new PersonName("Existing"),
            new PersonName("User"),
            _mockHasher.HashPassword("password"));
        _mockRepo.SeedUser(existingUser);

        // Act & Assert
        var isUnique = await _mockRepo.IsUsernameUniqueAsync(existingUsername);
        Assert.False(isUnique);
    }

    [Fact]
    public void RegisterUser_WithInvalidEmail_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new Email("invalid-email"));
    }

    [Fact]
    public void RegisterUser_WithInvalidUsername_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new Username("a")); // too short
    }

    [Fact]
    public void RegisterUser_WithInvalidFirstName_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new PersonName("")); // empty
    }

    [Fact]
    public void RegisterUser_WithInvalidLastName_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new PersonName(" ")); // whitespace only
    }

    [Fact]
    public void RegisterUser_WithInvalidPasswordHash_ShouldThrowUserDomainException()
    {
        // Arrange
        var email = new Email("test@example.com");
        var username = new Username("testuser");
        var firstName = new PersonName("Test");
        var lastName = new PersonName("User");

        // Act & Assert
        var exception = Assert.Throws<UserDomainException>(() => 
            User.Create(email, username, firstName, lastName, "")); // empty password hash
        
        Assert.IsType<UserDomainException>(exception);
        Assert.Equal("Password hash cannot be empty", exception.Message);
    }

    [Fact]
    public void RegisterUser_PasswordIsHashedCorrectly()
    {
        // Arrange
        var plainPassword = "MySecretPassword123!";
        
        // Act
        var hashedPassword = _mockHasher.HashPassword(plainPassword);
        var canVerify = _mockHasher.VerifyPassword(plainPassword, hashedPassword);
        var cannotVerifyWrong = _mockHasher.VerifyPassword("WrongPassword", hashedPassword);

        // Assert
        Assert.Equal("hash:MySecretPassword123!", hashedPassword);
        Assert.True(canVerify);
        Assert.False(cannotVerifyWrong);
    }

    [Fact]
    public async Task RegisterUser_MockRepositoryForceFlags_ShouldWorkCorrectly()
    {
        // Arrange
        var email = new Email("test@example.com");
        var username = new Username("testuser");

        // Initially unique
        Assert.True(await _mockRepo.IsEmailUniqueAsync(email));
        Assert.True(await _mockRepo.IsUsernameUniqueAsync(username));

        // Force non-unique
        _mockRepo.ForceEmailNotUnique = true;
        _mockRepo.ForceUsernameNotUnique = true;

        // Act & Assert
        Assert.False(await _mockRepo.IsEmailUniqueAsync(email));
        Assert.False(await _mockRepo.IsUsernameUniqueAsync(username));
    }
}
