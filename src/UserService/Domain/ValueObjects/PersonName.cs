using System;

namespace UserService.Domain.ValueObjects;

public record PersonName
{
    public string Value { get; }

    public PersonName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Name cannot be empty", nameof(value));
        
        var trimmed = value.Trim();
        if (trimmed.Length > 50)
            throw new ArgumentException("Name cannot exceed 50 characters", nameof(value));

        Value = trimmed;
    }

    public static implicit operator string(PersonName name) => name.Value;
    public static explicit operator PersonName(string value) => new(value);

    public override string ToString() => Value;
}
