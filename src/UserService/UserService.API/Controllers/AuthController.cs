using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Commands.LoginUser;
using UserService.Application.Commands.LogoutUser;
using UserService.Application.Commands.RegisterUser;
using UserService.API.Requests;

namespace UserService.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AuthRequest request)
    {
        var result = await _mediator.Send(new RegisterUserCommand(request.Name, request.Password));
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthRequest request)
    {
        var result = await _mediator.Send(new LoginUserCommand(request.Name, request.Password));
        return Ok(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        await _mediator.Send(new LogoutUserCommand(token));
        return Ok(new { message = "Logged out successfully." });
    }
}
