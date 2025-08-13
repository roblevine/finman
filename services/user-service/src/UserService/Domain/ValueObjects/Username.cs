using System;
using System.Text.RegularExpressions;

namespace UserService.Domain.ValueObjects;

public record Username
{
    private static readonly Regex UsernamePattern = new(@"^[a-zA-Z0-9_]{3,20}$", RegexOptions.Compiled);
    
    public string Value { get; }

    public Username(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Username cannot be empty", nameof(value));
        
        var trimmed = value.Trim();
        if (!UsernamePattern.IsMatch(trimmed))
            throw new ArgumentException("Username must be 3-20 characters, alphanumeric and underscore only", nameof(value));

        Value = trimmed.ToLowerInvariant();
    }

    public static implicit operator string(Username username) => username.Value;
    public static explicit operator Username(string value) => new(value);

    public override string ToString() => Value;
}
