using MediatR;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;
using UserService.Domain.Interfaces;

namespace UserService.Application.Commands.LoginUser;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;

    public LoginUserCommandHandler(
        IUserRepository userRepository,
        ITokenService tokenService,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthResultDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByNameAsync(request.Name, cancellationToken);

        if (user == null || !_passwordHasher.Verify(request.Password, user.Password))
            throw new UnauthorizedAccessException("Invalid credentials.");

        var token = _tokenService.GenerateToken(user);

        return new AuthResultDto(user.Id, user.Name, token);
    }
}
