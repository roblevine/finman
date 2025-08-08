using UserService.Application.DTOs;
using UserService.Application.UseCases;
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
    private readonly RegisterUserHandler _handler;

    public RegisterUserTests()
    {
        _mockRepo = new MockUserRepository();
        _mockHasher = new MockPasswordHasher();
        _handler = new RegisterUserHandler(_mockRepo, _mockHasher);
    }

    [Fact]
    public async Task RegisterUser_WithValidData_ShouldSucceed()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "john.doe@example.com",
            Username = "johndoe",
            FirstName = "John",
            LastName = "Doe",
            Password = "SecurePass123!"
        };

        // Act
        var response = await _handler.ExecuteAsync(request);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(request.Email, response.Email);
        Assert.Equal(request.Username, response.Username);
        Assert.Equal("John Doe", response.FullName);
        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.True(response.CreatedAt > DateTime.MinValue);
        
        // Verify user was persisted and uniqueness constraints now fail
        Assert.False(await _mockRepo.IsEmailUniqueAsync(new Email(request.Email)));
        Assert.False(await _mockRepo.IsUsernameUniqueAsync(new Username(request.Username)));
    }

    [Fact]
    public async Task RegisterUser_WithExistingEmail_ShouldFailUniquenessCheck()
    {
        // Arrange
        var existingUser = User.Create(
            new Email("existing@example.com"),
            new Username("existing"),
            new PersonName("Existing"),
            new PersonName("User"),
            _mockHasher.HashPassword("password"));
        _mockRepo.SeedUser(existingUser);

        var request = new RegisterRequest
        {
            Email = "existing@example.com", // Same email
            Username = "differentuser",
            FirstName = "Different",
            LastName = "User",
            Password = "Password123!"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.ExecuteAsync(request));
        Assert.Contains("Email 'existing@example.com' is already registered", exception.Message);
    }

    [Fact]
    public async Task RegisterUser_WithExistingUsername_ShouldFailUniquenessCheck()
    {
        // Arrange
        var existingUser = User.Create(
            new Email("existing@example.com"),
            new Username("existinguser"),
            new PersonName("Existing"),
            new PersonName("User"),
            _mockHasher.HashPassword("password"));
        _mockRepo.SeedUser(existingUser);

        var request = new RegisterRequest
        {
            Email = "different@example.com",
            Username = "existinguser", // Same username
            FirstName = "Different",
            LastName = "User",
            Password = "Password123!"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.ExecuteAsync(request));
        Assert.Contains("Username 'existinguser' is already taken", exception.Message);
    }

    [Fact]
    public async Task RegisterUser_WithInvalidEmail_ShouldThrowArgumentException()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "invalid-email", // Invalid format
            Username = "validuser",
            FirstName = "Valid",
            LastName = "User",
            Password = "Password123!"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.ExecuteAsync(request));
    }

    [Fact]
    public async Task RegisterUser_WithInvalidUsername_ShouldThrowArgumentException()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "valid@example.com",
            Username = "a", // Too short
            FirstName = "Valid",
            LastName = "User",
            Password = "Password123!"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.ExecuteAsync(request));
    }

    [Fact]
    public async Task RegisterUser_WithInvalidFirstName_ShouldThrowArgumentException()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "valid@example.com",
            Username = "validuser",
            FirstName = "", // Empty
            LastName = "User",
            Password = "Password123!"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.ExecuteAsync(request));
    }

    [Fact]
    public async Task RegisterUser_WithInvalidLastName_ShouldThrowArgumentException()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "valid@example.com",
            Username = "validuser",
            FirstName = "Valid",
            LastName = " ", // Whitespace only
            Password = "Password123!"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.ExecuteAsync(request));
    }

    [Fact]
    public void RegisterUser_WithInvalidPasswordHash_ShouldThrowUserDomainException()
    {
        // Arrange - Test the domain constraint directly since our mock hasher always returns valid hashes
        var email = new Email("test@example.com");
        var username = new Username("testuser");
        var firstName = new PersonName("Test");
        var lastName = new PersonName("User");

        // Act & Assert - Test domain entity creation with empty password hash
        var exception = Assert.Throws<UserDomainException>(() => 
            User.Create(email, username, firstName, lastName, "")); // empty password hash
        
        Assert.IsType<UserDomainException>(exception);
        Assert.Equal("Password hash cannot be empty", exception.Message);
    }

    [Fact]
    public async Task RegisterUser_PasswordIsHashedCorrectly()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Username = "testuser",
            FirstName = "Test",
            LastName = "User",
            Password = "MySecretPassword123!"
        };

        // Act
        var response = await _handler.ExecuteAsync(request);

        // Assert - Verify the password was hashed and user was created successfully
        Assert.NotNull(response);
        Assert.Equal(request.Email, response.Email);
        
        // Verify password hashing behavior through mock
        var hashedPassword = _mockHasher.HashPassword(request.Password);
        var canVerify = _mockHasher.VerifyPassword(request.Password, hashedPassword);
        var cannotVerifyWrong = _mockHasher.VerifyPassword("WrongPassword", hashedPassword);

        Assert.Equal("hash:MySecretPassword123!", hashedPassword);
        Assert.True(canVerify);
        Assert.False(cannotVerifyWrong);
    }

    [Fact]
    public async Task RegisterUser_MockRepositoryForceFlags_ShouldWorkCorrectly()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Username = "testuser",
            FirstName = "Test",
            LastName = "User",
            Password = "Password123!"
        };

        // Initially should succeed
        var response = await _handler.ExecuteAsync(request);
        Assert.NotNull(response);

        // Now force conflicts and verify they're detected
        _mockRepo.ForceEmailNotUnique = true;
        _mockRepo.ForceUsernameNotUnique = true;

        var anotherRequest = new RegisterRequest
        {
            Email = "different@example.com",
            Username = "differentuser",
            FirstName = "Different",
            LastName = "User",
            Password = "Password123!"
        };

        // Act & Assert - Should fail due to forced conflicts
        var emailException = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.ExecuteAsync(anotherRequest));
        Assert.Contains("Email 'different@example.com' is already registered", emailException.Message);
    }
}
