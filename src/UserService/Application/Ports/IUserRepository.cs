using System;
using System.Threading.Tasks;
using UserService.Domain.Entities;
using UserService.Domain.ValueObjects;

namespace UserService.Application.Ports;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(Email email);
    Task<User?> GetByUsernameAsync(Username username);
    Task<bool> IsEmailUniqueAsync(Email email);
    Task<bool> IsUsernameUniqueAsync(Username username);
    Task<User> AddAsync(User user);
    Task<User> UpdateAsync(User user);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<User>> GetAllActiveAsync();
}
