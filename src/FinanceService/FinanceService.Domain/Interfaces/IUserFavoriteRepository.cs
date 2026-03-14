using FinanceService.Domain.Entities;

namespace FinanceService.Domain.Interfaces;

public interface IUserFavoriteRepository
{
    Task AddAsync(UserFavorite favorite, CancellationToken ct = default);
    Task<bool> RemoveAsync(int userId, int currencyId, CancellationToken ct = default);
    Task<bool> ExistsAsync(int userId, int currencyId, CancellationToken ct = default);
}
