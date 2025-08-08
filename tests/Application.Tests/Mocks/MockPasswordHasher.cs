using UserService.Application.Ports;

namespace UserService.Tests.Application.Tests.Mocks;

/// <summary>
/// Deterministic password hasher for unit tests.
/// Hashes by prefixing a marker; NOT for production use.
/// </summary>
public class MockPasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        if (password is null) return "hash:null";
        return $"hash:{password}";
    }

    public bool VerifyPassword(string password, string hash)
    {
        return HashPassword(password) == hash;
    }
}
