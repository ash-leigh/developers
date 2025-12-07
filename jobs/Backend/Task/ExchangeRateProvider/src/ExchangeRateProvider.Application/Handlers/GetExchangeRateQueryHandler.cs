using ExchangeRateProvider.Application.Abstractions;
using ExchangeRateProvider.Application.DTOs;
using ExchangeRateProvider.Application.Queries;
using ExchangeRateProvider.Domain.Interfaces;
using ExchangeRateProvider.Domain.ValueObjects;

namespace ExchangeRateProvider.Application.Handlers;

public class GetExchangeRateQueryHandler(IExchangeRateService exchangeRateService) : IQueryHandler<GetExchangeRatesQuery, IList<ExchangeRateDto>>
{
    public async Task<IList<ExchangeRateDto>> HandleAsync(GetExchangeRatesQuery query, CancellationToken cancellationToken = default)
    {
        var rates = await exchangeRateService.GetExchangeRatesAsync(
            query.BaseCurrency,
            cancellationToken);

        IEnumerable<ExchangeRate> filteredRates = query.QuoteCurrencies.Count > 0
            ? rates.Where(r => query.QuoteCurrencies.Any(qc => qc.Code == r.QuoteCurrency.Code))
            : rates;

        return filteredRates.Select(r => new ExchangeRateDto(r.BaseCurrency, r.QuoteCurrency, r.Rate)).ToList();
    }
}
