using Moq;
using UserService.Application.Commands.LoginUser;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;

namespace UserService.Tests.Commands;

public class LoginUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly LoginUserCommandHandler _handler;

    public LoginUserCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _tokenServiceMock = new Mock<ITokenService>();
        _passwordHasherMock = new Mock<IPasswordHasher>();

        _handler = new LoginUserCommandHandler(
            _userRepositoryMock.Object,
            _tokenServiceMock.Object,
            _passwordHasherMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsAuthResultDto()
    {
        var user = new User { Id = 1, Name = "alice", Password = "hashed" };

        _userRepositoryMock
            .Setup(r => r.GetByNameAsync("alice", default))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(h => h.Verify("password123", "hashed"))
            .Returns(true);

        _tokenServiceMock
            .Setup(t => t.GenerateToken(user))
            .Returns("jwt.token");

        var result = await _handler.Handle(new LoginUserCommand("alice", "password123"), default);

        Assert.Equal(1, result.UserId);
        Assert.Equal("alice", result.UserName);
        Assert.Equal("jwt.token", result.Token);
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsUnauthorizedAccessException()
    {
        _userRepositoryMock
            .Setup(r => r.GetByNameAsync("unknown", default))
            .ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _handler.Handle(new LoginUserCommand("unknown", "pass"), default));
    }

    [Fact]
    public async Task Handle_WrongPassword_ThrowsUnauthorizedAccessException()
    {
        var user = new User { Id = 1, Name = "alice", Password = "hashed" };

        _userRepositoryMock
            .Setup(r => r.GetByNameAsync("alice", default))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(h => h.Verify("wrongpassword", "hashed"))
            .Returns(false);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _handler.Handle(new LoginUserCommand("alice", "wrongpassword"), default));
    }
}
