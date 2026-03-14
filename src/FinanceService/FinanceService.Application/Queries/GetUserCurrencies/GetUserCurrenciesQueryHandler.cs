using FinanceService.Application.DTOs;
using FinanceService.Domain.Interfaces;
using MediatR;

namespace FinanceService.Application.Queries.GetUserCurrencies;

public class GetUserCurrenciesQueryHandler : IRequestHandler<GetUserCurrenciesQuery, List<CurrencyDto>>
{
    private readonly ICurrencyRepository _currencyRepository;

    public GetUserCurrenciesQueryHandler(ICurrencyRepository currencyRepository)
    {
        _currencyRepository = currencyRepository;
    }

    public async Task<List<CurrencyDto>> Handle(GetUserCurrenciesQuery request, CancellationToken cancellationToken)
    {
        var currencies = await _currencyRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        return currencies
            .Select(c => new CurrencyDto(c.Id, c.Name, c.CharCode, c.Nominal, c.Rate))
            .ToList();
    }
}
