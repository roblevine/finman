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
    private readonly ConcurrentDictionary<Guid, User> _usersById = new();
    private readonly ConcurrentDictionary<string, User> _usersByEmail = new();
    private readonly ConcurrentDictionary<string, User> _usersByUsername = new();
    private readonly object _lockObject = new();

    /// <summary>
    /// Checks if a user with the given email address already exists.
    /// </summary>
    /// <param name="email">The email address to check for uniqueness</param>
    /// <returns>True if a user with this email exists, false otherwise</returns>
    public Task<bool> ExistsByEmailAsync(Email email)
    {
        ArgumentNullException.ThrowIfNull(email);
        
        // Use lock for consistency with AddAsync operations
        lock (_lockObject)
        {
            // Check if email exists in the repository
            bool exists = _usersByEmail.ContainsKey(email.Value);
            return Task.FromResult(exists);
        }
    }

    /// <summary>
    /// Checks if a user with the given username already exists.
    /// </summary>
    /// <param name="username">The username to check for uniqueness</param>
    /// <returns>True if a user with this username exists, false otherwise</returns>
    public Task<bool> ExistsByUsernameAsync(Username username)
    {
        ArgumentNullException.ThrowIfNull(username);
        
        // Use lock for consistency with AddAsync operations
        lock (_lockObject)
        {
            // Check if username exists in the repository
            bool exists = _usersByUsername.ContainsKey(username.Value);
            return Task.FromResult(exists);
        }
    }

    /// <summary>
    /// Adds a new user to the repository.
    /// </summary>
    /// <param name="user">The user entity to add</param>
    /// <exception cref="InvalidOperationException">Thrown when a user with the same email or username already exists</exception>
    public Task AddAsync(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        // Get the email and username values for dictionary keys
        var emailKey = user.Email.Value;
        var usernameKey = user.Username.Value;

        // Use lock to ensure atomic operations across all dictionaries
        lock (_lockObject)
        {
            // Check for duplicate email
            if (_usersByEmail.ContainsKey(emailKey))
            {
                throw new InvalidOperationException($"A user with email '{emailKey}' already exists.");
            }

            // Check for duplicate username
            if (_usersByUsername.ContainsKey(usernameKey))
            {
                throw new InvalidOperationException($"A user with username '{usernameKey}' already exists.");
            }

            // Add to all dictionaries
            _usersById[user.Id] = user;
            _usersByEmail[emailKey] = user;
            _usersByUsername[usernameKey] = user;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user</param>
    /// <returns>The user entity if found, null otherwise</returns>
    public Task<User?> GetByIdAsync(Guid id)
    {
        lock (_lockObject)
        {
            _usersById.TryGetValue(id, out User? user);
            return Task.FromResult(user);
        }
    }

    /// <summary>
    /// Finds a user by their email address.
    /// </summary>
    /// <param name="email">The email address to search for</param>
    /// <returns>The user entity if found, null otherwise</returns>
    public Task<User?> FindByEmailAsync(Email email)
    {
        ArgumentNullException.ThrowIfNull(email);
        
        lock (_lockObject)
        {
            _usersByEmail.TryGetValue(email.Value, out User? user);
            return Task.FromResult(user);
        }
    }
}
