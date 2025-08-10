using System;
using System.Threading.Tasks;
using UserService.Domain.Entities;
using UserService.Domain.ValueObjects;
using UserService.Infrastructure.Repositories;
using Xunit;

namespace UserService.Tests.Infrastructure.Tests.Repositories;

/// <summary>
/// Tests for InMemoryUserRepository to ensure thread-safe operations,
/// uniqueness constraints, and proper domain entity integration.
/// </summary>
public class InMemoryUserRepositoryTests
{
    private readonly InMemoryUserRepository _repository;

    public InMemoryUserRepositoryTests()
    {
        _repository = new InMemoryUserRepository();
    }

    [Fact]
    public async Task IsEmailUniqueAsync_WithNewEmail_ShouldReturnTrue()
    {
        // Arrange
        var email = new Email("unique@example.com");

        // Act
        var isUnique = await _repository.IsEmailUniqueAsync(email);

        // Assert
        Assert.True(isUnique);
    }

    [Fact]
    public async Task IsEmailUniqueAsync_WithExistingEmail_ShouldReturnFalse()
    {
        // Arrange
        var email = new Email("existing@example.com");
        var username = new Username("testuser");
        var firstName = new PersonName("Test");
        var lastName = new PersonName("User");
        var user = User.Create(email, username, firstName, lastName, "hashedPassword");
        
        await _repository.AddAsync(user);

        // Act
        var isUnique = await _repository.IsEmailUniqueAsync(email);

        // Assert
        Assert.False(isUnique);
    }

    [Fact]
    public async Task IsUsernameUniqueAsync_WithNewUsername_ShouldReturnTrue()
    {
        // Arrange
        var username = new Username("uniqueuser");

        // Act
        var isUnique = await _repository.IsUsernameUniqueAsync(username);

        // Assert
        Assert.True(isUnique);
    }

    [Fact]
    public async Task IsUsernameUniqueAsync_WithExistingUsername_ShouldReturnFalse()
    {
        // Arrange
        var email = new Email("test@example.com");
        var username = new Username("existinguser");
        var firstName = new PersonName("Test");
        var lastName = new PersonName("User");
        var user = User.Create(email, username, firstName, lastName, "hashedPassword");
        
        await _repository.AddAsync(user);

        // Act
        var isUnique = await _repository.IsUsernameUniqueAsync(username);

        // Assert
        Assert.False(isUnique);
    }

    [Fact]
    public async Task AddAsync_WithValidUser_ShouldStoreUser()
    {
        // Arrange
        var email = new Email("new@example.com");
        var username = new Username("newuser");
        var firstName = new PersonName("New");
        var lastName = new PersonName("User");
        var user = User.Create(email, username, firstName, lastName, "hashedPassword");

        // Act
        var result = await _repository.AddAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Email.Value, result.Email.Value);
        Assert.Equal(user.Username.Value, result.Username.Value);

        // Verify it's actually stored by checking uniqueness
        var emailUnique = await _repository.IsEmailUniqueAsync(email);
        var usernameUnique = await _repository.IsUsernameUniqueAsync(username);
        
        Assert.False(emailUnique);
        Assert.False(usernameUnique);
    }

    [Fact]
    public async Task AddAsync_WithDuplicateEmail_ShouldUpdateExistingUser()
    {
        // Arrange
        var email = new Email("duplicate@example.com");
        var username1 = new Username("user1");
        var username2 = new Username("user2");
        var firstName = new PersonName("Test");
        var lastName = new PersonName("User");

        var user1 = User.Create(email, username1, firstName, lastName, "hashedPassword1");
        var user2 = User.Create(email, username2, firstName, lastName, "hashedPassword2");

        // Act
        await _repository.AddAsync(user1);
        var result = await _repository.AddAsync(user2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user2.Id, result.Id);
        Assert.Equal(user2.Username.Value, result.Username.Value);

        // Verify the second user overwrote the first for the email
        var emailUnique = await _repository.IsEmailUniqueAsync(email);
        var username1Unique = await _repository.IsUsernameUniqueAsync(username1);
        var username2Unique = await _repository.IsUsernameUniqueAsync(username2);
        
        Assert.False(emailUnique);
        Assert.True(username1Unique);   // user1's username should be available again
        Assert.False(username2Unique);  // user2's username should be taken
    }

    [Fact]
    public async Task ConcurrentOperations_ShouldBeThreadSafe()
    {
        // Arrange
        var tasks = new Task[100];
        var baseEmail = "concurrent";
        var baseUsername = "user";

        // Act - Create 100 concurrent users
        for (int i = 0; i < 100; i++)
        {
            var index = i;
            tasks[i] = Task.Run(async () =>
            {
                var email = new Email($"{baseEmail}{index}@example.com");
                var username = new Username($"{baseUsername}{index}");
                var firstName = new PersonName("Concurrent");
                var lastName = new PersonName("User");
                var user = User.Create(email, username, firstName, lastName, $"hash{index}");
                
                await _repository.AddAsync(user);
            });
        }

        await Task.WhenAll(tasks);

        // Assert - Verify all users were stored correctly
        for (int i = 0; i < 100; i++)
        {
            var email = new Email($"{baseEmail}{i}@example.com");
            var username = new Username($"{baseUsername}{i}");
            
            var emailUnique = await _repository.IsEmailUniqueAsync(email);
            var usernameUnique = await _repository.IsUsernameUniqueAsync(username);
            
            Assert.False(emailUnique, $"Email {email.Value} should not be unique after adding");
            Assert.False(usernameUnique, $"Username {username.Value} should not be unique after adding");
        }
    }

    [Fact]
    public async Task ConcurrentUniquenessChecks_ShouldBeConsistent()
    {
        // Arrange
        var email = new Email("consistency@example.com");
        var username = new Username("consistencyuser");
        var firstName = new PersonName("Consistency");
        var lastName = new PersonName("Test");
        var user = User.Create(email, username, firstName, lastName, "hashedPassword");

        await _repository.AddAsync(user);

        // Act - Run 50 concurrent uniqueness checks
        var emailTasks = new Task<bool>[50];
        var usernameTasks = new Task<bool>[50];

        for (int i = 0; i < 50; i++)
        {
            emailTasks[i] = _repository.IsEmailUniqueAsync(email);
            usernameTasks[i] = _repository.IsUsernameUniqueAsync(username);
        }

        var emailResults = await Task.WhenAll(emailTasks);
        var usernameResults = await Task.WhenAll(usernameTasks);

        // Assert - All checks should consistently return false
        foreach (var result in emailResults)
        {
            Assert.False(result, "All concurrent email uniqueness checks should return false");
        }

        foreach (var result in usernameResults)
        {
            Assert.False(result, "All concurrent username uniqueness checks should return false");
        }
    }

    [Fact]
    public async Task EmailCaseInsensitivity_ShouldBeHandledCorrectly()
    {
        // Arrange
        var emailLower = new Email("case@example.com");
        var emailUpper = new Email("CASE@EXAMPLE.COM");  // Email constructor normalizes to lowercase
        var username = new Username("caseuser");
        var firstName = new PersonName("Case");
        var lastName = new PersonName("Test");

        var user = User.Create(emailLower, username, firstName, lastName, "hashedPassword");
        await _repository.AddAsync(user);

        // Act & Assert
        var lowerUnique = await _repository.IsEmailUniqueAsync(emailLower);
        var upperUnique = await _repository.IsEmailUniqueAsync(emailUpper);

        // Both should be false since Email value object normalizes to lowercase
        Assert.False(lowerUnique);
        Assert.False(upperUnique);
        Assert.Equal(emailLower.Value, emailUpper.Value); // Verify they're the same after normalization
    }

    [Fact]
    public async Task UsernameCaseInsensitivity_ShouldBeHandledCorrectly()
    {
        // Arrange
        var email = new Email("username@example.com");
        var usernameLower = new Username("caseuser");
        var usernameUpper = new Username("CASEUSER");  // Username constructor normalizes to lowercase
        var firstName = new PersonName("Case");
        var lastName = new PersonName("Test");

        var user = User.Create(email, usernameLower, firstName, lastName, "hashedPassword");
        await _repository.AddAsync(user);

        // Act & Assert
        var lowerUnique = await _repository.IsUsernameUniqueAsync(usernameLower);
        var upperUnique = await _repository.IsUsernameUniqueAsync(usernameUpper);

        // Both should be false since Username value object normalizes to lowercase
        Assert.False(lowerUnique);
        Assert.False(upperUnique);
        Assert.Equal(usernameLower.Value, usernameUpper.Value); // Verify they're the same after normalization
    }
}
