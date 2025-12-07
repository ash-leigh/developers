using ExchangeRateProvider.Domain.Interfaces;
using ExchangeRateProvider.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeRateProvider.Infrastructure.ExternalServices;

public class ExchangeRateService(HttpClient httpClient, ILogger<ExchangeRateService> logger) : IExchangeRateService
{
    public async Task<IList<ExchangeRate>>GetExchangeRatesAsync(Currency baseCurrency, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync(new Uri("https://api.cnb.cz/cnbapi/exrates/daily?lang=EN"), cancellationToken);
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        var root = await JsonSerializer.DeserializeAsync<ExchangeRateResponse>(stream, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }, cancellationToken).ConfigureAwait(false);
        var list = root?.Rates ?? new List<CnbRate>();

        return list.Select(r => new ExchangeRate(
            baseCurrency,
            new Currency(r.CurrencyCode),
            r.Amount / r.Rate
        )).ToList();
    }
}
