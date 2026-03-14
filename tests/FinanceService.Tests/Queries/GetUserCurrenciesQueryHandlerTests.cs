using FinanceService.Application.Queries.GetUserCurrencies;
using FinanceService.Domain.Entities;
using FinanceService.Domain.Interfaces;
using Moq;

namespace FinanceService.Tests.Queries;

public class GetUserCurrenciesQueryHandlerTests
{
    private readonly Mock<ICurrencyRepository> _currencyRepositoryMock;
    private readonly GetUserCurrenciesQueryHandler _handler;

    public GetUserCurrenciesQueryHandlerTests()
    {
        _currencyRepositoryMock = new Mock<ICurrencyRepository>();

        _handler = new GetUserCurrenciesQueryHandler(_currencyRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_UserWithFavorites_ReturnsCurrencies()
    {
        var currencies = new List<Currency>
        {
            new() { Id = 10, Name = "Доллар США", CharCode = "USD", Nominal = 1, Rate = 90.5m },
            new() { Id = 20, Name = "Евро",       CharCode = "EUR", Nominal = 1, Rate = 99.2m }
        };

        _currencyRepositoryMock
            .Setup(r => r.GetByUserIdAsync(1, default))
            .ReturnsAsync(currencies);

        var result = await _handler.Handle(new GetUserCurrenciesQuery(1), default);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, c => c.CharCode == "USD" && c.Rate == 90.5m);
        Assert.Contains(result, c => c.CharCode == "EUR" && c.Rate == 99.2m);
    }

    [Fact]
    public async Task Handle_UserWithNoFavorites_ReturnsEmptyList()
    {
        _currencyRepositoryMock
            .Setup(r => r.GetByUserIdAsync(42, default))
            .ReturnsAsync(new List<Currency>());

        var result = await _handler.Handle(new GetUserCurrenciesQuery(42), default);

        Assert.Empty(result);
        _currencyRepositoryMock.Verify(r => r.GetByUserIdAsync(42, default), Times.Once);
    }
}
