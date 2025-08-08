using System;
using System.Threading.Tasks;
using UserService.Domain.Entities;
using UserService.Domain.ValueObjects;

namespace UserService.Application.Ports;

public interface IUserRepository
{
    Task<bool> IsEmailUniqueAsync(Email email);
    Task<bool> IsUsernameUniqueAsync(Username username);
    Task<User> AddAsync(User user);
}
