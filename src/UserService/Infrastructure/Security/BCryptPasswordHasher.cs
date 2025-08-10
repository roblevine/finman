using BCrypt.Net;
using UserService.Application.Ports;

namespace UserService.Infrastructure.Security;

/// <summary>
/// BCrypt-based implementation of password hashing for secure password storage.
/// Uses BCrypt.Net-Next library with secure defaults for work factor and salt generation.
/// </summary>
public class BCryptPasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12; // Recommended work factor for 2025, ~300ms on modern hardware

    /// <summary>
    /// Hashes a password using BCrypt with a randomly generated salt.
    /// Uses work factor of 12 for optimal security/performance balance.
    /// </summary>
    /// <param name="password">The password to hash. Cannot be null, empty, or whitespace.</param>
    /// <returns>A BCrypt hash string that includes the salt and work factor.</returns>
    /// <exception cref="ArgumentException">Thrown when password is null, empty, or whitespace.</exception>
    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password cannot be null, empty, or whitespace.", nameof(password));
        }

        // BCrypt.HashPassword automatically generates a random salt and includes it in the hash
        // The work factor determines the computational cost (2^workFactor iterations)
        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }

    /// <summary>
    /// Verifies a password against a BCrypt hash.
    /// Safely handles invalid hashes and null/empty passwords without throwing exceptions.
    /// </summary>
    /// <param name="password">The plaintext password to verify.</param>
    /// <param name="hash">The BCrypt hash to verify against.</param>
    /// <returns>True if the password matches the hash, false otherwise.</returns>
    public bool VerifyPassword(string password, string hash)
    {
        // Handle null or empty inputs gracefully
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
        {
            return false;
        }

        try
        {
            // BCrypt.Verify handles all the complexity of extracting salt and work factor from hash
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch
        {
            // If hash is malformed or any other error occurs, return false rather than throwing
            // This prevents information leakage and provides consistent behavior
            return false;
        }
    }
}
