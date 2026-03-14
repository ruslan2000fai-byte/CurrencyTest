using FinanceService.Domain.Entities;

namespace FinanceService.Domain.Interfaces;

public interface ICurrencyRepository
{
    Task<Currency?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<Currency>> GetByUserIdAsync(int userId, CancellationToken ct = default);
    Task<List<Currency>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken ct = default);
    Task<List<Currency>> GetAllAsync(CancellationToken ct = default);
}
