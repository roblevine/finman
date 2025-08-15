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
    private readonly ConcurrentDictionary<Guid, User> _byId = new();
    private readonly ConcurrentDictionary<string, User> _byEmail = new();
    private readonly ConcurrentDictionary<string, User> _byUsername = new();

    public bool ForceEmailExists { get; set; }
    public bool ForceUsernameExists { get; set; }

    public Task<bool> ExistsByEmailAsync(Email email)
    {
        if (ForceEmailExists) return Task.FromResult(true);
        return Task.FromResult(_byEmail.ContainsKey(email.Value));
    }

    public Task<bool> ExistsByUsernameAsync(Username username)
    {
        if (ForceUsernameExists) return Task.FromResult(true);
        return Task.FromResult(_byUsername.ContainsKey(username.Value));
    }

    public Task AddAsync(User user)
    {
        _byId[user.Id] = user;
        _byEmail[user.Email.Value] = user;
        _byUsername[user.Username.Value] = user;
        return Task.CompletedTask;
    }

    public Task<User?> GetByIdAsync(Guid id)
    {
        _byId.TryGetValue(id, out User? user);
        return Task.FromResult(user);
    }

    public Task<User?> FindByEmailAsync(Email email)
    {
        _byEmail.TryGetValue(email.Value, out User? user);
        return Task.FromResult(user);
    }

    // Helpers to pre-seed state for tests
    public void SeedUser(User user)
    {
        _byId[user.Id] = user;
        _byEmail[user.Email.Value] = user;
        _byUsername[user.Username.Value] = user;
    }
}
