using MediatR;

namespace FinanceService.Application.Commands.RemoveFavoriteCurrency;

public record RemoveFavoriteCurrencyCommand(int UserId, int CurrencyId) : IRequest<bool>;
