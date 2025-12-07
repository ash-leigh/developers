namespace ExchangeRateProvider.Domain.ValueObjects;

public record Currency
{
    public Currency(string code)
    {
        Code = code;
    }

    public string Code { get; }
}
