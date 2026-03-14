using FinanceService.Application.DTOs;
using MediatR;

namespace FinanceService.Application.Queries.GetUserCurrencies;

public record GetUserCurrenciesQuery(int UserId) : IRequest<List<CurrencyDto>>;
