using ExchangeRateProvider.Api.Constants;
using ExchangeRateProvider.Application.Abstractions;
using ExchangeRateProvider.Application.DTOs;
using ExchangeRateProvider.Application.Queries;
using ExchangeRateProvider.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeRateProvider.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Tags("Exchange Rates")]
    public class ExchangeRateController(IQueryHandler<GetExchangeRatesQuery, IList<ExchangeRateDto>> handler, ILogger<ExchangeRateController> logger) : ControllerBase
    {
        [HttpGet(ApiEndpoints.ExchangeRates.GetByCurrency)]
        [ProducesResponseType<IEnumerable<ExchangeRateDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Get exchange rates")]
        [EndpointDescription("Retrieves current exchange rates for the specified base currency, with optional filtering by quote currencies.")]
        public async Task<IEnumerable<ExchangeRateDto>> GetByCurrency(
            [FromQuery] string baseCurrency,
            [FromQuery] List<string> quoteCurrencies,
            CancellationToken cancellationToken = default)
        {
            var quotes = quoteCurrencies?
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(code => new Currency(code.ToUpperInvariant()))
                .ToList() ?? new List<Currency>();

            var rates = await handler.HandleAsync(new GetExchangeRatesQuery(new Currency(baseCurrency), quotes), CancellationToken.None);
            return rates;
        }
    }
}
