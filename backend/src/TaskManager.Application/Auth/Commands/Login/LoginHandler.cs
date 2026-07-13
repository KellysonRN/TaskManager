using TaskManager.Application.Auth.Contracts;
using TaskManager.Application.Common.Exceptions;
using TaskManager.Application.Contracts;

namespace TaskManager.Application.Auth.Commands.Login;

public class LoginHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<LoginResult> HandleAsync(LoginCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Email))
            throw new ValidationException("Email is required");

        if (string.IsNullOrWhiteSpace(command.Password))
            throw new ValidationException("Password is required");

        var user = await _userRepository.FindByEmailAsync(command.Email, cancellationToken);

        // Same message for "not found" and "wrong password" to avoid user enumeration (OWASP A07)
        if (user is null)
            throw new UnauthorizedException("Invalid credentials");

        if (!_passwordHasher.Verify(command.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid credentials");

        var token = _tokenService.CreateToken(user.Id.ToString(), Array.Empty<string>());

        return new LoginResult { Token = token };
    }
}
