using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // 1. Реєструємо AutoMapper
        services.AddAutoMapper(cfg => cfg.AddMaps(assembly));

        // 2. Реєструємо MediatR    Це навчить програму шукати всі Handlers у нашому проекті Application
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // 3. Реєстрація всіх валідаторів
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
