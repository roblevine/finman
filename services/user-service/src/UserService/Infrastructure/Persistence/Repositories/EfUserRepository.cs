using Microsoft.EntityFrameworkCore;
using UserService.Application.Ports;
using UserService.Domain.Entities;
using UserService.Domain.ValueObjects;

namespace UserService.Infrastructure.Persistence.Repositories;

/// <summary>
/// Entity Framework Core implementation of the IUserRepository interface.
/// Provides persistence operations for User entities using PostgreSQL with proper
/// value object handling and database constraint enforcement.
/// </summary>
public class EfUserRepository : IUserRepository
{
    private readonly FinmanDbContext _context;

    public EfUserRepository(FinmanDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Checks if a user with the given email address already exists.
    /// Uses case-insensitive comparison via PostgreSQL citext.
    /// </summary>
    /// <param name="email">The email address to check for uniqueness</param>
    /// <returns>True if a user with this email exists, false otherwise</returns>
    public async Task<bool> ExistsByEmailAsync(Email email)
    {
        ArgumentNullException.ThrowIfNull(email);
        
        return await _context.Users
            .AnyAsync(u => u.Email.Value == email.Value);
    }

    /// <summary>
    /// Checks if a user with the given username already exists.
    /// Uses case-insensitive comparison via PostgreSQL citext.
    /// </summary>
    /// <param name="username">The username to check for uniqueness</param>
    /// <returns>True if a user with this username exists, false otherwise</returns>
    public async Task<bool> ExistsByUsernameAsync(Username username)
    {
        ArgumentNullException.ThrowIfNull(username);
        
        return await _context.Users
            .AnyAsync(u => u.Username.Value == username.Value);
    }

    /// <summary>
    /// Adds a new user to the repository.
    /// </summary>
    /// <param name="user">The user entity to add</param>
    /// <exception cref="InvalidOperationException">Thrown when a user with the same email or username already exists</exception>
    public async Task AddAsync(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        try
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            // Map database unique constraint violations to domain exceptions
            if (ex.Message.Contains("email") || ex.Message.Contains("users_email_key"))
            {
                throw new InvalidOperationException($"A user with email '{user.Email.Value}' already exists.", ex);
            }
            
            if (ex.Message.Contains("username") || ex.Message.Contains("users_username_key"))
            {
                throw new InvalidOperationException($"A user with username '{user.Username.Value}' already exists.", ex);
            }
            
            // Re-throw if it's a different constraint violation
            throw new InvalidOperationException("A database constraint violation occurred while adding the user.", ex);
        }
    }

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user</param>
    /// <returns>The user entity if found, null otherwise</returns>
    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    /// <summary>
    /// Finds a user by their email address.
    /// Uses case-insensitive comparison via PostgreSQL citext.
    /// </summary>
    /// <param name="email">The email address to search for</param>
    /// <returns>The user entity if found, null otherwise</returns>
    public async Task<User?> FindByEmailAsync(Email email)
    {
        ArgumentNullException.ThrowIfNull(email);
        
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email.Value == email.Value);
    }

    /// <summary>
    /// Determines if a DbUpdateException represents a unique constraint violation.
    /// </summary>
    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        // PostgreSQL unique constraint violation error codes and patterns
        return ex.InnerException?.Message.Contains("duplicate key value violates unique constraint") == true ||
               ex.InnerException?.Message.Contains("23505") == true; // PostgreSQL unique violation error code
    }
}
