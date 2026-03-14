using FinanceService.Application.Commands.AddFavoriteCurrency;
using FinanceService.Domain.Entities;
using FinanceService.Domain.Interfaces;
using Moq;

namespace FinanceService.Tests.Commands;

public class AddFavoriteCurrencyCommandHandlerTests
{
    private readonly Mock<IUserFavoriteRepository> _favoriteRepositoryMock;
    private readonly Mock<ICurrencyRepository> _currencyRepositoryMock;
    private readonly AddFavoriteCurrencyCommandHandler _handler;

    public AddFavoriteCurrencyCommandHandlerTests()
    {
        _favoriteRepositoryMock = new Mock<IUserFavoriteRepository>();
        _currencyRepositoryMock = new Mock<ICurrencyRepository>();

        _handler = new AddFavoriteCurrencyCommandHandler(
            _favoriteRepositoryMock.Object,
            _currencyRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_NewFavorite_AddsAndReturnsTrue()
    {
        _favoriteRepositoryMock
            .Setup(r => r.ExistsAsync(1, 10, default))
            .ReturnsAsync(false);

        _currencyRepositoryMock
            .Setup(r => r.GetByIdAsync(10, default))
            .ReturnsAsync(new Currency { Id = 10, Name = "USD", CharCode = "USD", Nominal = 1, Rate = 90m });

        var result = await _handler.Handle(new AddFavoriteCurrencyCommand(1, 10), default);

        Assert.True(result);
        _favoriteRepositoryMock.Verify(r => r.AddAsync(It.Is<UserFavorite>(f => f.UserId == 1 && f.CurrencyId == 10), default), Times.Once);
    }

    [Fact]
    public async Task Handle_AlreadyExists_ReturnsFalse()
    {
        _favoriteRepositoryMock
            .Setup(r => r.ExistsAsync(1, 10, default))
            .ReturnsAsync(true);

        var result = await _handler.Handle(new AddFavoriteCurrencyCommand(1, 10), default);

        Assert.False(result);
        _favoriteRepositoryMock.Verify(r => r.AddAsync(It.IsAny<UserFavorite>(), default), Times.Never);
    }

    [Fact]
    public async Task Handle_CurrencyNotFound_ThrowsKeyNotFoundException()
    {
        _favoriteRepositoryMock
            .Setup(r => r.ExistsAsync(1, 999, default))
            .ReturnsAsync(false);

        _currencyRepositoryMock
            .Setup(r => r.GetByIdAsync(999, default))
            .ReturnsAsync((Currency?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(new AddFavoriteCurrencyCommand(1, 999), default));
    }
}
