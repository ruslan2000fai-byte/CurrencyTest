namespace FinanceService.Domain.Entities;

public class RevokedToken
{
    public string Jti { get; set; } = string.Empty;
    public DateTime RevokedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
