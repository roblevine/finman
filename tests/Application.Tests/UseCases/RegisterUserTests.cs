using UserService.Domain.ValueObjects;
using UserService.Domain.Entities;
using UserService.Domain.Exceptions;
using UserService.Tests.Application.Tests.Mocks;
using Xunit;
using FluentAssertions;

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
        addedUser.Should().NotBeNull();
        addedUser.Email.Should().Be(email);
        addedUser.Username.Should().Be(username);
        addedUser.FirstName.Should().Be(firstName);
        addedUser.LastName.Should().Be(lastName);
        addedUser.PasswordHash.Should().Be("hash:SecurePass123!");
        
        // Verify uniqueness checks would have passed
        (await _mockRepo.IsEmailUniqueAsync(email)).Should().BeFalse(); // now taken
        (await _mockRepo.IsUsernameUniqueAsync(username)).Should().BeFalse(); // now taken
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
        isUnique.Should().BeFalse();
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
        isUnique.Should().BeFalse();
    }

    [Fact]
    public void RegisterUser_WithInvalidEmail_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var act = () => new Email("invalid-email");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void RegisterUser_WithInvalidUsername_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var act = () => new Username("a"); // too short
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void RegisterUser_WithInvalidFirstName_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var act = () => new PersonName(""); // empty
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void RegisterUser_WithInvalidLastName_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var act = () => new PersonName(" "); // whitespace only
        act.Should().Throw<ArgumentException>();
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
        
        exception.Should().BeOfType<UserDomainException>()
            .Which.Message.Should().Be("Password hash cannot be empty");
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
        hashedPassword.Should().Be("hash:MySecretPassword123!");
        canVerify.Should().BeTrue();
        cannotVerifyWrong.Should().BeFalse();
    }

    [Fact]
    public async Task RegisterUser_MockRepositoryForceFlags_ShouldWorkCorrectly()
    {
        // Arrange
        var email = new Email("test@example.com");
        var username = new Username("testuser");

        // Initially unique
        (await _mockRepo.IsEmailUniqueAsync(email)).Should().BeTrue();
        (await _mockRepo.IsUsernameUniqueAsync(username)).Should().BeTrue();

        // Force non-unique
        _mockRepo.ForceEmailNotUnique = true;
        _mockRepo.ForceUsernameNotUnique = true;

        // Act & Assert
        (await _mockRepo.IsEmailUniqueAsync(email)).Should().BeFalse();
        (await _mockRepo.IsUsernameUniqueAsync(username)).Should().BeFalse();
    }
}
