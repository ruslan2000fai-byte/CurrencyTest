namespace FinanceService.Application.DTOs;

public record CurrencyDto(int Id, string Name, string CharCode, int Nominal, decimal Rate);
