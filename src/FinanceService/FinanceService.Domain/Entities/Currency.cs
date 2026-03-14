namespace FinanceService.Domain.Entities;

public class Currency
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CharCode { get; set; } = string.Empty;
    public int Nominal { get; set; } = 1;
    public decimal Rate { get; set; }
}
