using UserService.Infrastructure.Security;
using Xunit;

namespace UserService.Tests.Infrastructure.Tests.Security;

public class BCryptPasswordHasherTests
{
    private readonly BCryptPasswordHasher _hasher;

    public BCryptPasswordHasherTests()
    {
        _hasher = new BCryptPasswordHasher();
    }

    [Fact]
    public void HashPassword_WithValidPassword_ShouldReturnHashedPassword()
    {
        // Arrange
        var password = "SecurePassword123!";

        // Act
        var hash = _hasher.HashPassword(password);

        // Assert
        Assert.NotNull(hash);
        Assert.NotEmpty(hash);
        Assert.NotEqual(password, hash); // Hash should not equal plaintext
        Assert.True(hash.Length > 50); // BCrypt hashes are typically 60+ characters
        Assert.StartsWith("$2", hash); // BCrypt hashes start with $2a, $2b, or $2y
    }

    [Fact]
    public void HashPassword_WithSamePassword_ShouldReturnDifferentHashes()
    {
        // Arrange
        var password = "SamePassword123!";

        // Act
        var hash1 = _hasher.HashPassword(password);
        var hash2 = _hasher.HashPassword(password);

        // Assert
        Assert.NotEqual(hash1, hash2); // Each hash should have a unique salt
    }

    [Fact]
    public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        var password = "CorrectPassword123!";
        var hash = _hasher.HashPassword(password);

        // Act
        var isValid = _hasher.VerifyPassword(password, hash);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void VerifyPassword_WithIncorrectPassword_ShouldReturnFalse()
    {
        // Arrange
        var correctPassword = "CorrectPassword123!";
        var incorrectPassword = "WrongPassword123!";
        var hash = _hasher.HashPassword(correctPassword);

        // Act
        var isValid = _hasher.VerifyPassword(incorrectPassword, hash);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void VerifyPassword_WithEmptyPassword_ShouldReturnFalse()
    {
        // Arrange
        var originalPassword = "TestPassword123!";
        var hash = _hasher.HashPassword(originalPassword);

        // Act
        var isValid = _hasher.VerifyPassword("", hash);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void VerifyPassword_WithNullPassword_ShouldReturnFalse()
    {
        // Arrange
        var originalPassword = "TestPassword123!";
        var hash = _hasher.HashPassword(originalPassword);

        // Act
        var isValid = _hasher.VerifyPassword(null!, hash);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void VerifyPassword_WithInvalidHash_ShouldReturnFalse()
    {
        // Arrange
        var password = "TestPassword123!";
        var invalidHash = "invalid-hash-format";

        // Act
        var isValid = _hasher.VerifyPassword(password, invalidHash);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void HashPassword_WithNullPassword_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _hasher.HashPassword(null!));
        Assert.Contains("password", exception.Message.ToLower());
    }

    [Fact]
    public void HashPassword_WithEmptyPassword_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _hasher.HashPassword(""));
        Assert.Contains("password", exception.Message.ToLower());
    }

    [Fact]
    public void HashPassword_WithWhitespacePassword_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _hasher.HashPassword("   "));
        Assert.Contains("password", exception.Message.ToLower());
    }

    [Fact]
    public void HashPassword_WithComplexPassword_ShouldWorkCorrectly()
    {
        // Arrange - Test with various special characters and Unicode
        var complexPassword = "Tëst!@#$%^&*()_+{}|:<>?[];',./`~Pässwörđ123";

        // Act
        var hash = _hasher.HashPassword(complexPassword);
        var isValid = _hasher.VerifyPassword(complexPassword, hash);

        // Assert
        Assert.NotNull(hash);
        Assert.True(isValid);
    }

    [Fact]
    public void HashPassword_WithVeryLongPassword_ShouldWorkCorrectly()
    {
        // Arrange - BCrypt can handle passwords up to 72 bytes
        var longPassword = new string('A', 70) + "!2"; // 72 characters

        // Act
        var hash = _hasher.HashPassword(longPassword);
        var isValid = _hasher.VerifyPassword(longPassword, hash);

        // Assert
        Assert.NotNull(hash);
        Assert.True(isValid);
    }
}
