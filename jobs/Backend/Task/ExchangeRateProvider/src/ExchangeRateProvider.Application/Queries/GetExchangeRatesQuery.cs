using ExchangeRateProvider.Domain.ValueObjects;

namespace ExchangeRateProvider.Application.Queries;

public record GetExchangeRatesQuery(
    Currency BaseCurrency,
    List<Currency>? QuoteCurrencies = null
);
