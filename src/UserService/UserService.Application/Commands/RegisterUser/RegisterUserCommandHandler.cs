using MediatR;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;

namespace UserService.Application.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        ITokenService tokenService,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthResultDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.ExistsByNameAsync(request.Name, cancellationToken))
            throw new InvalidOperationException($"User '{request.Name}' already exists.");

        var user = new User
        {
            Name = request.Name,
            Password = _passwordHasher.Hash(request.Password)
        };

        var created = await _userRepository.CreateAsync(user, cancellationToken);
        var token = _tokenService.GenerateToken(created);

        return new AuthResultDto(created.Id, created.Name, token);
    }
}
