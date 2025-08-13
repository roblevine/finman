using System.Collections.Concurrent;
using UserService.Application.Ports;
using UserService.Domain.Entities;
using UserService.Domain.ValueObjects;

namespace UserService.Infrastructure.Repositories;

/// <summary>
/// In-memory implementation of the user repository interface.
/// Uses ConcurrentDictionary for thread-safe operations with proper synchronization.
/// </summary>
public class InMemoryUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<string, User> _usersByEmail = new();
    private readonly ConcurrentDictionary<string, User> _usersByUsername = new();
    private readonly object _lockObject = new();

    /// <summary>
    /// Checks if the given email is unique (not already in use).
    /// </summary>
    /// <param name="email">The email to check for uniqueness.</param>
    /// <returns>True if the email is unique, false if it's already in use.</returns>
    public Task<bool> IsEmailUniqueAsync(Email email)
    {
        ArgumentNullException.ThrowIfNull(email);
        
        // Use lock for consistency with AddAsync operations
        lock (_lockObject)
        {
            // Check if email exists in the repository
            bool isUnique = !_usersByEmail.ContainsKey(email.Value);
            return Task.FromResult(isUnique);
        }
    }

    /// <summary>
    /// Checks if the given username is unique (not already in use).
    /// </summary>
    /// <param name="username">The username to check for uniqueness.</param>
    /// <returns>True if the username is unique, false if it's already in use.</returns>
    public Task<bool> IsUsernameUniqueAsync(Username username)
    {
        ArgumentNullException.ThrowIfNull(username);
        
        // Use lock for consistency with AddAsync operations
        lock (_lockObject)
        {
            // Check if username exists in the repository
            bool isUnique = !_usersByUsername.ContainsKey(username.Value);
            return Task.FromResult(isUnique);
        }
    }

    /// <summary>
    /// Adds a new user to the repository or updates an existing user.
    /// When adding, both email and username keys are updated simultaneously
    /// to maintain consistency between the two lookup dictionaries.
    /// Uses locking to ensure atomic updates across both dictionaries.
    /// </summary>
    /// <param name="user">The user to add to the repository.</param>
    /// <returns>The added user.</returns>
    public Task<User> AddAsync(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        // Get the email and username values for dictionary keys
        var emailKey = user.Email.Value;
        var usernameKey = user.Username.Value;

        // Use lock to ensure atomic operations across both dictionaries
        lock (_lockObject)
        {
            // Check if there's an existing user with this email
            if (_usersByEmail.TryGetValue(emailKey, out var existingUserByEmail))
            {
                // Remove the old username entry for the existing user
                _usersByUsername.TryRemove(existingUserByEmail.Username.Value, out _);
            }

            // Check if there's an existing user with this username
            if (_usersByUsername.TryGetValue(usernameKey, out var existingUserByUsername))
            {
                // Remove the old email entry for the existing user
                _usersByEmail.TryRemove(existingUserByUsername.Email.Value, out _);
            }

            // Add/update both dictionaries with the new user
            _usersByEmail.AddOrUpdate(emailKey, user, (key, oldUser) => user);
            _usersByUsername.AddOrUpdate(usernameKey, user, (key, oldUser) => user);
        }

        return Task.FromResult(user);
    }
}
