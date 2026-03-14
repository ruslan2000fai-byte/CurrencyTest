using FinanceService.Application.Commands.RemoveFavoriteCurrency;
using FinanceService.Domain.Interfaces;
using Moq;

namespace FinanceService.Tests.Commands;

public class RemoveFavoriteCurrencyCommandHandlerTests
{
    private readonly Mock<IUserFavoriteRepository> _favoriteRepositoryMock;
    private readonly RemoveFavoriteCurrencyCommandHandler _handler;

    public RemoveFavoriteCurrencyCommandHandlerTests()
    {
        _favoriteRepositoryMock = new Mock<IUserFavoriteRepository>();
        _handler = new RemoveFavoriteCurrencyCommandHandler(_favoriteRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingFavorite_RemovesAndReturnsTrue()
    {
        _favoriteRepositoryMock
            .Setup(r => r.RemoveAsync(1, 10, default))
            .ReturnsAsync(true);

        var result = await _handler.Handle(new RemoveFavoriteCurrencyCommand(1, 10), default);

        Assert.True(result);
        _favoriteRepositoryMock.Verify(r => r.RemoveAsync(1, 10, default), Times.Once);
    }

    [Fact]
    public async Task Handle_FavoriteNotFound_ReturnsFalse()
    {
        _favoriteRepositoryMock
            .Setup(r => r.RemoveAsync(1, 999, default))
            .ReturnsAsync(false);

        var result = await _handler.Handle(new RemoveFavoriteCurrencyCommand(1, 999), default);

        Assert.False(result);
    }
}
