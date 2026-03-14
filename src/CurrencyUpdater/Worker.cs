using System.Globalization;
using System.Xml.Linq;
using Microsoft.Extensions.Http;
using Npgsql;

namespace CurrencyUpdater;

public class CurrencyWorker : BackgroundService
{
    private readonly ILogger<CurrencyWorker> _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    private const string CbrUrl = "http://www.cbr.ru/scripts/XML_daily.asp";
    private static readonly TimeSpan UpdateInterval = TimeSpan.FromHours(1);

    public CurrencyWorker(ILogger<CurrencyWorker> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _configuration = configuration;
        _httpClient = httpClientFactory.CreateClient("cbr");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("CurrencyWorker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await UpdateCurrenciesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating currencies.");
            }

            await Task.Delay(UpdateInterval, stoppingToken);
        }
    }

    private async Task UpdateCurrenciesAsync(CancellationToken ct)
    {
        _logger.LogInformation("Fetching currency data from CBR...");

        var bytes = await _httpClient.GetByteArrayAsync(CbrUrl, ct);

        // CBR returns XML in Windows-1251 encoding
        var encoding = System.Text.Encoding.GetEncoding("windows-1251");
        var xml = encoding.GetString(bytes);

        var doc = XDocument.Parse(xml);

        var currencies = doc.Root?
            .Elements("Valute")
            .Select(v => new
            {
                Name = v.Element("Name")?.Value ?? string.Empty,
                CharCode = v.Element("CharCode")?.Value ?? string.Empty,
                Nominal = int.Parse(v.Element("Nominal")?.Value ?? "1"),
                Rate = decimal.Parse(
                    v.Element("Value")?.Value?.Replace(",", ".") ?? "0",
                    CultureInfo.InvariantCulture)
            })
            .Where(c => !string.IsNullOrEmpty(c.CharCode))
            .ToList();

        if (currencies == null || currencies.Count == 0)
        {
            _logger.LogWarning("No currencies parsed from CBR response.");
            return;
        }

        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync(ct);

        foreach (var currency in currencies)
        {
            const string upsert = """
                INSERT INTO currency (name, char_code, nominal, rate)
                VALUES (@name, @charCode, @nominal, @rate)
                ON CONFLICT (char_code) DO UPDATE
                    SET name    = EXCLUDED.name,
                        nominal = EXCLUDED.nominal,
                        rate    = EXCLUDED.rate;
                """;

            await using var cmd = new NpgsqlCommand(upsert, conn);
            cmd.Parameters.AddWithValue("name", currency.Name);
            cmd.Parameters.AddWithValue("charCode", currency.CharCode);
            cmd.Parameters.AddWithValue("nominal", currency.Nominal);
            cmd.Parameters.AddWithValue("rate", currency.Rate);
            await cmd.ExecuteNonQueryAsync(ct);
        }

        _logger.LogInformation("Successfully updated {Count} currencies at {Time}.", currencies.Count, DateTimeOffset.UtcNow);
    }
}
