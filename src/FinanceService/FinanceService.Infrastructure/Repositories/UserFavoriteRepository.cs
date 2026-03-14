using FinanceService.Domain.Entities;
using FinanceService.Domain.Interfaces;
using FinanceService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceService.Infrastructure.Repositories;

public class UserFavoriteRepository : IUserFavoriteRepository
{
    private readonly AppDbContext _context;

    public UserFavoriteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(UserFavorite favorite, CancellationToken ct = default)
    {
        _context.UserFavorites.Add(favorite);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<bool> RemoveAsync(int userId, int currencyId, CancellationToken ct = default)
    {
        var favorite = await _context.UserFavorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.CurrencyId == currencyId, ct);

        if (favorite == null)
            return false;

        _context.UserFavorites.Remove(favorite);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> ExistsAsync(int userId, int currencyId, CancellationToken ct = default)
        => await _context.UserFavorites
            .AnyAsync(f => f.UserId == userId && f.CurrencyId == currencyId, ct);
}
