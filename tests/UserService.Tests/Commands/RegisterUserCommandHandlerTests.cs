using Moq;
using UserService.Application.Commands.RegisterUser;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;

namespace UserService.Tests.Commands;

public class RegisterUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _tokenServiceMock = new Mock<ITokenService>();
        _passwordHasherMock = new Mock<IPasswordHasher>();

        _handler = new RegisterUserCommandHandler(
            _userRepositoryMock.Object,
            _tokenServiceMock.Object,
            _passwordHasherMock.Object);
    }

    [Fact]
    public async Task Handle_NewUser_ReturnsAuthResultDto()
    {
        _userRepositoryMock
            .Setup(r => r.ExistsByNameAsync("alice", default))
            .ReturnsAsync(false);

        _passwordHasherMock
            .Setup(h => h.Hash("password123"))
            .Returns("hashedpassword");

        _userRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<User>(), default))
            .ReturnsAsync(new User { Id = 1, Name = "alice", Password = "hashedpassword" });

        _tokenServiceMock
            .Setup(t => t.GenerateToken(It.IsAny<User>()))
            .Returns("jwt.token.here");

        var result = await _handler.Handle(new RegisterUserCommand("alice", "password123"), default);

        Assert.Equal(1, result.UserId);
        Assert.Equal("alice", result.UserName);
        Assert.Equal("jwt.token.here", result.Token);
    }

    [Fact]
    public async Task Handle_ExistingUser_ThrowsInvalidOperationException()
    {
        _userRepositoryMock
            .Setup(r => r.ExistsByNameAsync("alice", default))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(new RegisterUserCommand("alice", "password123"), default));
    }

    [Fact]
    public async Task Handle_NewUser_HashesPasswordBeforeCreating()
    {
        _userRepositoryMock
            .Setup(r => r.ExistsByNameAsync(It.IsAny<string>(), default))
            .ReturnsAsync(false);

        _passwordHasherMock
            .Setup(h => h.Hash("rawpassword"))
            .Returns("bcrypt_hash");

        User? capturedUser = null;
        _userRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<User>(), default))
            .Callback<User, CancellationToken>((u, _) => capturedUser = u)
            .ReturnsAsync(new User { Id = 2, Name = "bob", Password = "bcrypt_hash" });

        _tokenServiceMock
            .Setup(t => t.GenerateToken(It.IsAny<User>()))
            .Returns("token");

        await _handler.Handle(new RegisterUserCommand("bob", "rawpassword"), default);

        Assert.NotNull(capturedUser);
        Assert.Equal("bcrypt_hash", capturedUser!.Password);
        Assert.NotEqual("rawpassword", capturedUser.Password);
    }
}
