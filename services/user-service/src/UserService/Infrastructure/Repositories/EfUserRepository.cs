using Microsoft.EntityFrameworkCore;
using UserService.Application.Ports;
using UserService.Domain.Entities;
using UserService.Domain.ValueObjects;
using UserService.Infrastructure.Persistence;

namespace UserService.Infrastructure.Repositories;

public class EfUserRepository : IUserRepository
{
    private readonly FinmanDbContext _db;

    public EfUserRepository(FinmanDbContext db)
    {
        _db = db;
    }

    public async Task<bool> IsEmailUniqueAsync(Email email)
    {
        if (email == null) throw new ArgumentNullException(nameof(email));
        return !await _db.Users.AsNoTracking().AnyAsync(u => u.Email.Value == email.Value);
    }

    public async Task<bool> IsUsernameUniqueAsync(Username username)
    {
        if (username == null) throw new ArgumentNullException(nameof(username));
        return !await _db.Users.AsNoTracking().AnyAsync(u => u.Username.Value == username.Value);
    }

    public async Task<User> AddAsync(User user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }
}
