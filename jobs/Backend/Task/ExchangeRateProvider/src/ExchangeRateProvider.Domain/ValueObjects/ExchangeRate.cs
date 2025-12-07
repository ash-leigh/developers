namespace ExchangeRateProvider.Domain.ValueObjects;

public record ExchangeRate
{
    public ExchangeRate(Currency baseCurrency, Currency quoteCurrency, decimal rate)
    {
        BaseCurrency = baseCurrency;
        QuoteCurrency = quoteCurrency;
        Rate = rate;
    }

    public Currency BaseCurrency { get; }

    public Currency QuoteCurrency { get; }

    public decimal Rate { get; }
}
