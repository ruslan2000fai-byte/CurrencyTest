using FinanceService.Domain.Entities;
using FinanceService.Domain.Interfaces;
using MediatR;

namespace FinanceService.Application.Commands.AddFavoriteCurrency;

public class AddFavoriteCurrencyCommandHandler : IRequestHandler<AddFavoriteCurrencyCommand, bool>
{
    private readonly IUserFavoriteRepository _favoriteRepository;
    private readonly ICurrencyRepository _currencyRepository;

    public AddFavoriteCurrencyCommandHandler(
        IUserFavoriteRepository favoriteRepository,
        ICurrencyRepository currencyRepository)
    {
        _favoriteRepository = favoriteRepository;
        _currencyRepository = currencyRepository;
    }

    public async Task<bool> Handle(AddFavoriteCurrencyCommand request, CancellationToken cancellationToken)
    {
        if (await _favoriteRepository.ExistsAsync(request.UserId, request.CurrencyId, cancellationToken))
            return false;

        var currency = await _currencyRepository.GetByIdAsync(request.CurrencyId, cancellationToken);
        if (currency == null)
            throw new KeyNotFoundException($"Currency with id {request.CurrencyId} not found.");

        await _favoriteRepository.AddAsync(
            new UserFavorite { UserId = request.UserId, CurrencyId = request.CurrencyId },
            cancellationToken);

        return true;
    }
}
