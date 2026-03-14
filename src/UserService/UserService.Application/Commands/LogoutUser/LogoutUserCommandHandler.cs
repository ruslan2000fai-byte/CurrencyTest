using MediatR;
using UserService.Application.Interfaces;

namespace UserService.Application.Commands.LogoutUser;

public class LogoutUserCommandHandler : IRequestHandler<LogoutUserCommand, bool>
{
    private readonly ITokenService _tokenService;

    public LogoutUserCommandHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task<bool> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
        => await _tokenService.RevokeTokenAsync(request.Token, cancellationToken);
}
