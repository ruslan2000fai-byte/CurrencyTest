using Moq;
using UserService.Application.Commands.LogoutUser;
using UserService.Application.Interfaces;

namespace UserService.Tests.Commands;

public class LogoutUserCommandHandlerTests
{
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly LogoutUserCommandHandler _handler;

    public LogoutUserCommandHandlerTests()
    {
        _tokenServiceMock = new Mock<ITokenService>();
        _handler = new LogoutUserCommandHandler(_tokenServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidToken_RevokesTokenAndReturnsTrue()
    {
        _tokenServiceMock
            .Setup(t => t.RevokeTokenAsync("valid.jwt.token", default))
            .ReturnsAsync(true);

        var result = await _handler.Handle(new LogoutUserCommand("valid.jwt.token"), default);

        Assert.True(result);
        _tokenServiceMock.Verify(t => t.RevokeTokenAsync("valid.jwt.token", default), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidToken_ReturnsFalse()
    {
        _tokenServiceMock
            .Setup(t => t.RevokeTokenAsync("invalid", default))
            .ReturnsAsync(false);

        var result = await _handler.Handle(new LogoutUserCommand("invalid"), default);

        Assert.False(result);
    }
}
