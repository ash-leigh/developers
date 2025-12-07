using ExchangeRateProvider.Application.Abstractions;
using ExchangeRateProvider.Application.DTOs;
using ExchangeRateProvider.Application.Handlers;
using ExchangeRateProvider.Application.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace ExchangeRateProvider.Application;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<GetExchangeRatesQuery, IList<ExchangeRateDto>>, GetExchangeRateQueryHandler>();

        return services;
    }
}
