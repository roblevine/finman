using System;
using UserService.Domain.ValueObjects;
using UserService.Domain.Exceptions;

namespace UserService.Domain.Entities;

public class User
{
    // Private constructor for Entity Framework
    private User() { }

    // Static factory method for better control
    public static User Create(
        Email email,
        Username username,
        PersonName firstName,
        PersonName lastName,
        string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new UserDomainException("Password hash cannot be empty");

        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Username = username,
            FirstName = firstName,
            LastName = lastName,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true,
            IsDeleted = false
        };
    }

    public Guid Id { get; private set; }
    public Email Email { get; private set; } = null!;
    public Username Username { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!; // TODO: Move to Auth service later
    public PersonName FirstName { get; private set; } = null!;
    public PersonName LastName { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    public string FullName => $"{FirstName} {LastName}";

    public void UpdateProfile(PersonName firstName, PersonName lastName)
    {
        FirstName = firstName;
        LastName = lastName;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new UserDomainException("Password hash cannot be empty");

        PasswordHash = newPasswordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        UpdatedAt = DateTime.UtcNow;
    }
}
