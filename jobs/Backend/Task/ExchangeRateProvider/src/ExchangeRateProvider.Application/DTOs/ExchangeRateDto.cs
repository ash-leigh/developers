using ExchangeRateProvider.Domain.ValueObjects;

namespace ExchangeRateProvider.Application.DTOs;

public record ExchangeRateDto(
    Currency BaseCurrency,
    Currency QuoteCurrency,
    decimal Rate
);
