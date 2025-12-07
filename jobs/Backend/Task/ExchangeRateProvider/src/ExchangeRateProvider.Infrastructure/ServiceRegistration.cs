using ExchangeRateProvider.Domain.Interfaces;
using ExchangeRateProvider.Infrastructure.ExternalServices;
using Microsoft.Extensions.DependencyInjection;

namespace ExchangeRateProvider.Infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddHttpClient<IExchangeRateService, ExchangeRateService>(client =>
        {
            client.BaseAddress = new Uri("https://api.cnb.cz");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        return services;
    }
}
