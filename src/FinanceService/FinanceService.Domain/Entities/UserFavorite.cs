namespace FinanceService.Domain.Entities;

public class UserFavorite
{
    public int UserId { get; set; }
    public int CurrencyId { get; set; }
    public Currency Currency { get; set; } = null!;
}
