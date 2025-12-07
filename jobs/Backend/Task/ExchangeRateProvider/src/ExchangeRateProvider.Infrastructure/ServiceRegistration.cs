using ExchangeRateProvider.Domain.Constants;
using ExchangeRateProvider.Domain.Interfaces;
using ExchangeRateProvider.Infrastructure.ExternalServices.CZK;
using ExchangeRateProvider.Infrastructure.Factories;
using Microsoft.Extensions.DependencyInjection;

namespace ExchangeRateProvider.Infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IExchangeRateServiceFactory, ExchangeRateServiceFactory>();

        services.AddHttpClient<CzkExchangeRateService>(client =>
        {
            client.BaseAddress = new Uri("https://api.cnb.cz");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddKeyedTransient<IExchangeRateService, CzkExchangeRateService>(CurrencyServiceKeys.CZK);

        return services;
    }
}
