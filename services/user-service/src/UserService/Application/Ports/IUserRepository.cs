using UserService.Domain.Entities;
using UserService.Domain.ValueObjects;

namespace UserService.Application.Ports;

/// <summary>
/// Repository port for User aggregate persistence operations.
/// Defines the contract for user data access without exposing infrastructure concerns.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Checks if a user with the given email address already exists.
    /// </summary>
    /// <param name="email">The email address to check for uniqueness</param>
    /// <returns>True if a user with this email exists, false otherwise</returns>
    Task<bool> ExistsByEmailAsync(Email email);

    /// <summary>
    /// Checks if a user with the given username already exists.
    /// </summary>
    /// <param name="username">The username to check for uniqueness</param>
    /// <returns>True if a user with this username exists, false otherwise</returns>
    Task<bool> ExistsByUsernameAsync(Username username);

    /// <summary>
    /// Adds a new user to the repository.
    /// </summary>
    /// <param name="user">The user entity to add</param>
    /// <exception cref="InvalidOperationException">Thrown when a user with the same email or username already exists</exception>
    Task AddAsync(User user);

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user</param>
    /// <returns>The user entity if found, null otherwise</returns>
    Task<User?> GetByIdAsync(Guid id);

    /// <summary>
    /// Finds a user by their email address.
    /// </summary>
    /// <param name="email">The email address to search for</param>
    /// <returns>The user entity if found, null otherwise</returns>
    Task<User?> FindByEmailAsync(Email email);
}
