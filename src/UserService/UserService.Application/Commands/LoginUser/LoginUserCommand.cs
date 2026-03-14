using MediatR;
using UserService.Application.DTOs;

namespace UserService.Application.Commands.LoginUser;

public record LoginUserCommand(string Name, string Password) : IRequest<AuthResultDto>;
