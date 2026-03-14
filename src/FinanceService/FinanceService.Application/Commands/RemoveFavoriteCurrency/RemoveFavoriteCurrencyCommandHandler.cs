using FinanceService.Domain.Interfaces;
using MediatR;

namespace FinanceService.Application.Commands.RemoveFavoriteCurrency;

public class RemoveFavoriteCurrencyCommandHandler : IRequestHandler<RemoveFavoriteCurrencyCommand, bool>
{
    private readonly IUserFavoriteRepository _favoriteRepository;

    public RemoveFavoriteCurrencyCommandHandler(IUserFavoriteRepository favoriteRepository)
    {
        _favoriteRepository = favoriteRepository;
    }

    public async Task<bool> Handle(RemoveFavoriteCurrencyCommand request, CancellationToken cancellationToken)
    {
        return await _favoriteRepository.RemoveAsync(request.UserId, request.CurrencyId, cancellationToken);
    }
}
