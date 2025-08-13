using System.Collections.Concurrent;
using UserService.Application.Ports;
using UserService.Domain.Entities;
using UserService.Domain.ValueObjects;

namespace UserService.Tests.Application.Tests.Mocks;

/// <summary>
/// Simple in-memory mock implementing IUserRepository for unit tests.
/// Provides configurable behavior for uniqueness checks and AddAsync.
/// </summary>
public class MockUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<string, User> _byEmail = new();
    private readonly ConcurrentDictionary<string, User> _byUsername = new();

    public bool ForceEmailNotUnique { get; set; }
    public bool ForceUsernameNotUnique { get; set; }

    public Task<bool> IsEmailUniqueAsync(Email email)
    {
        if (ForceEmailNotUnique) return Task.FromResult(false);
        return Task.FromResult(!_byEmail.ContainsKey(email.Value));
    }

    public Task<bool> IsUsernameUniqueAsync(Username username)
    {
        if (ForceUsernameNotUnique) return Task.FromResult(false);
        return Task.FromResult(!_byUsername.ContainsKey(username.Value));
    }

    public Task<User> AddAsync(User user)
    {
        _byEmail[user.Email.Value] = user;
        _byUsername[user.Username.Value] = user;
        return Task.FromResult(user);
    }

    // Helpers to pre-seed state for tests
    public void SeedUser(User user)
    {
        _byEmail[user.Email.Value] = user;
        _byUsername[user.Username.Value] = user;
    }
}
