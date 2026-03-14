using FinanceService.Domain.Entities;
using FinanceService.Domain.Interfaces;
using FinanceService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceService.Infrastructure.Repositories;

public class CurrencyRepository : ICurrencyRepository
{
    private readonly AppDbContext _context;

    public CurrencyRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Currency?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _context.Currencies.FindAsync(new object[] { id }, ct);

    public async Task<List<Currency>> GetByUserIdAsync(int userId, CancellationToken ct = default)
        => await _context.Currencies
            .Where(c => _context.UserFavorites.Any(f => f.UserId == userId && f.CurrencyId == c.Id))
            .ToListAsync(ct);

    public async Task<List<Currency>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken ct = default)
        => await _context.Currencies.Where(c => ids.Contains(c.Id)).ToListAsync(ct);

    public async Task<List<Currency>> GetAllAsync(CancellationToken ct = default)
        => await _context.Currencies.ToListAsync(ct);
}
