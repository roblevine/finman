using System;

namespace UserService.Domain.ValueObjects;

public record Email
{
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty", nameof(value));
        
        var trimmed = value.Trim();
        if (!IsValidEmailFormat(trimmed))
            throw new ArgumentException("Invalid email format", nameof(value));

        Value = trimmed.ToLowerInvariant();
    }

    private static bool IsValidEmailFormat(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public static implicit operator string(Email email) => email.Value;
    public static explicit operator Email(string value) => new(value);

    public override string ToString() => Value;
}
