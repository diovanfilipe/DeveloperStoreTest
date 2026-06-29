using DeveloperStore.SalesApi.Application.Abstractions;
using DeveloperStore.SalesApi.Infrastructure.Logging;
using DeveloperStore.SalesApi.Infrastructure.Repository;
using DeveloperStore.SalesApi.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DeveloperStore.SalesApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<SqliteConnectionProvider>();
        services.AddSingleton<DatabaseInitializer>();
        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddScoped<IEventPublisher, LogEventPublisher>();
        return services;
    }
}
