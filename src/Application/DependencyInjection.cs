using Application.Common.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Реєструємо AutoMapper
        services.AddAutoMapper(cfg => cfg.AddMaps(assembly));

        // Реєструємо MediatR    Це навчить програму шукати всі Handlers у нашому проекті Application
        services.AddMediatR(cfg => 
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        // Реєстрація всіх валідаторів
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
