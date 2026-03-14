using MediatR;

namespace FinanceService.Application.Commands.AddFavoriteCurrency;

public record AddFavoriteCurrencyCommand(int UserId, int CurrencyId) : IRequest<bool>;
