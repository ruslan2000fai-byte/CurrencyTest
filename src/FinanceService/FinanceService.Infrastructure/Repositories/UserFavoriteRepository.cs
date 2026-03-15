using FinanceService.Domain.Entities;
using FinanceService.Domain.Interfaces;
using FinanceService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceService.Infrastructure.Repositories;

public class UserFavoriteRepository : IUserFavoriteRepository
{
    private readonly AppReadDbContext _readContext;
    private readonly AppWriteDbContext _writeContext;

    public UserFavoriteRepository(AppReadDbContext readContext, AppWriteDbContext writeContext)
    {
        _readContext = readContext;
        _writeContext = writeContext;
    }

    public async Task AddAsync(UserFavorite favorite, CancellationToken ct = default)
    {
        _writeContext.UserFavorites.Add(favorite);
        await _writeContext.SaveChangesAsync(ct);
    }

    public async Task<bool> RemoveAsync(int userId, int currencyId, CancellationToken ct = default)
    {
        var favorite = await _readContext.UserFavorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.CurrencyId == currencyId, ct);

        if (favorite == null)
            return false;

        _writeContext.UserFavorites.Remove(favorite);
        await _writeContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> ExistsAsync(int userId, int currencyId, CancellationToken ct = default)
        => await _readContext.UserFavorites
            .AnyAsync(f => f.UserId == userId && f.CurrencyId == currencyId, ct);
}
