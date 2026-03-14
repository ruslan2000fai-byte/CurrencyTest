using MediatR;

namespace UserService.Application.Commands.LogoutUser;

public record LogoutUserCommand(string Token) : IRequest<bool>;
