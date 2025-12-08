using ExchangeRateProvider.Domain.Interfaces;
using ExchangeRateProvider.Domain.ValueObjects;
using ExchangeRateProvider.Infrastructure.Policies;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Registry;
using System.Text.Json;

namespace ExchangeRateProvider.Infrastructure.ExternalServices.CZK;

public class CzkExchangeRateService(HttpClient httpClient, IReadOnlyPolicyRegistry<string> policyRegistry, ILogger<CzkExchangeRateService> logger) : IExchangeRateService
{
    public async Task<IList<ExchangeRate>>GetExchangeRatesAsync(Currency baseCurrency, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching CZK exchange rates from external service.");

        var retryPolicy = policyRegistry.Get<IAsyncPolicy<HttpResponseMessage>>(PolicyNames.WaitAndRetry) ?? Policy.NoOpAsync<HttpResponseMessage>();
        var context = new Context($"{nameof(GetExchangeRatesAsync)}-{Guid.NewGuid()}", new Dictionary<string, object>
        {
            { PolicyContextItems.Logger, logger }
        });

        var response = await retryPolicy.ExecuteAsync(ctx => httpClient.GetAsync(new Uri("https://api.cnb.cz/cnbapi/exrates/daily?lang=EN")), context);
        response.EnsureSuccessStatusCode();
        
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        var root = await JsonSerializer.DeserializeAsync<CzkExchangeRateResponse>(stream, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }, cancellationToken).ConfigureAwait(false);
        var list = root?.Rates ?? new List<CzkRate>();

        return list.Select(r => new ExchangeRate(
            baseCurrency,
            new Currency(r.CurrencyCode),
            r.Amount / r.Rate
        )).ToList();
    }
}
