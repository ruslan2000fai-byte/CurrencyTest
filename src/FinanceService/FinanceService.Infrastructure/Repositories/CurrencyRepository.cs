using FinanceService.Domain.Entities;
using FinanceService.Domain.Interfaces;
using FinanceService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceService.Infrastructure.Repositories;

public class CurrencyRepository : ICurrencyRepository
{
    private readonly AppReadDbContext _readContext;
    private readonly AppWriteDbContext _writeContext;

    public CurrencyRepository(AppReadDbContext readContext, AppWriteDbContext writeContext)
    {
        _readContext = readContext;
        _writeContext = writeContext;
    }

    public async Task<Currency?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _readContext.Currencies.FindAsync(new object[] { id }, ct);

    public async Task<List<Currency>> GetByUserIdAsync(int userId, CancellationToken ct = default)
        => await _readContext.Currencies
            .Where(c => _readContext.UserFavorites.Any(f => f.UserId == userId && f.CurrencyId == c.Id))
            .ToListAsync(ct);

    public async Task<List<Currency>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken ct = default)
        => await _readContext.Currencies.Where(c => ids.Contains(c.Id)).ToListAsync(ct);

    public async Task<List<Currency>> GetAllAsync(CancellationToken ct = default)
        => await _readContext.Currencies.ToListAsync(ct);
}
