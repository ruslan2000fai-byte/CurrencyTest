using MediatR;
using UserService.Application.DTOs;

namespace UserService.Application.Commands.RegisterUser;

public record RegisterUserCommand(string Name, string Password) : IRequest<AuthResultDto>;
