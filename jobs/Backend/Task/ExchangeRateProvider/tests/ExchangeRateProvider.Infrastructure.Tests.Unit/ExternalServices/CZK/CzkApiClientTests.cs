using ExchangeRateProvider.Infrastructure.ExternalServices.CZK;
using ExchangeRateProvider.Infrastructure.Policies;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Polly;
using Polly.Registry;
using Shouldly;
using System.Net;
using System.Text.Json;

namespace ExchangeRateProvider.Infrastructure.Tests.Unit.ExternalServices.CZK;

public class CzkApiClientTests
{
    private readonly IReadOnlyPolicyRegistry<string> _policyRegistry;
    private readonly FakeLogger<CzkApiClient> _logger;

    public CzkApiClientTests()
    {
        _policyRegistry = new PolicyRegistry
        {
            [PolicyNames.WaitAndRetry] = Policy.NoOpAsync<HttpResponseMessage>()
        };
        _logger = new FakeLogger<CzkApiClient>();
    }

    [Fact]
    public async Task GetExchangeRatesAsync_WithSuccessfulResponse_ReturnsDeserializedData()
    {
        // Arrange
        var response = new CzkExchangeRateResponse(new List<CzkRate>
        {
            new() { Amount = 1, CurrencyCode = "USD", Rate = 23.5m, Country = "USA", Currency = "Dollar", Order = 1, ValidFor = DateOnly.FromDateTime(DateTime.Today) }
        });
        var httpClient = CreateHttpClientWithResponse(HttpStatusCode.OK, response);
        var client = new CzkApiClient(httpClient, _policyRegistry, _logger);

        // Act
        var result = await client.GetExchangeRatesAsync(CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Rates.ShouldHaveSingleItem();
        result.Rates[0].CurrencyCode.ShouldBe("USD");
        
        var logs = _logger.Collector.GetSnapshot();
        logs.Count.ShouldBe(2);
        logs[0].Level.ShouldBe(LogLevel.Information);
        logs[0].Message.ShouldContain("Fetching exchange rates from CNB API");
        logs[1].Level.ShouldBe(LogLevel.Information);
        logs[1].Message.ShouldContain("Successfully fetched exchange rates from CNB API");
    }

    [Fact]
    public async Task GetExchangeRatesAsync_WithHttpError_ThrowsHttpRequestException()
    {
        // Arrange
        var httpClient = CreateHttpClientWithResponse(HttpStatusCode.InternalServerError, string.Empty);
        var client = new CzkApiClient(httpClient, _policyRegistry, _logger);

        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(() => client.GetExchangeRatesAsync(CancellationToken.None));
        
        var logs = _logger.Collector.GetSnapshot();
        logs[0].Level.ShouldBe(LogLevel.Information);
        logs[0].Message.ShouldContain("Fetching exchange rates from CNB API");
    }

    private static HttpClient CreateHttpClientWithResponse(HttpStatusCode statusCode, object content)
    {
        var json = content is string s ? s : JsonSerializer.Serialize(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        var handler = new MockHttpMessageHandler(statusCode, json);
        return new HttpClient(handler) { BaseAddress = new Uri("https://api.cnb.cz") };
    }

    private class MockHttpMessageHandler(HttpStatusCode statusCode, string content) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(content)
            });
        }
    }
}
