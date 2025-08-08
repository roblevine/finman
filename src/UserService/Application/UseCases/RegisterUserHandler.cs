using UserService.Application.DTOs;
using UserService.Application.Ports;
using UserService.Domain.Entities;
using UserService.Domain.ValueObjects;
using UserService.Domain.Exceptions;

namespace UserService.Application.UseCases;

public class RegisterUserHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
    }

    public async Task<RegisterResponse> ExecuteAsync(RegisterRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Create value objects (will throw ArgumentException for invalid inputs)
        var email = new Email(request.Email);
        var username = new Username(request.Username);
        var firstName = new PersonName(request.FirstName);
        var lastName = new PersonName(request.LastName);

        // Check uniqueness constraints
        var isEmailUnique = await _userRepository.IsEmailUniqueAsync(email);
        if (!isEmailUnique)
        {
            throw new InvalidOperationException($"Email '{email.Value}' is already registered");
        }

        var isUsernameUnique = await _userRepository.IsUsernameUniqueAsync(username);
        if (!isUsernameUnique)
        {
            throw new InvalidOperationException($"Username '{username.Value}' is already taken");
        }

        // Hash password
        var hashedPassword = _passwordHasher.HashPassword(request.Password);

        // Create domain entity (will throw UserDomainException for invalid password hash)
        var user = User.Create(email, username, firstName, lastName, hashedPassword);

        // Persist user
        var savedUser = await _userRepository.AddAsync(user);

        // Return response
        return new RegisterResponse
        {
            Id = savedUser.Id,
            Email = savedUser.Email.Value,
            Username = savedUser.Username.Value,
            FullName = savedUser.FullName,
            CreatedAt = savedUser.CreatedAt
        };
    }
}
