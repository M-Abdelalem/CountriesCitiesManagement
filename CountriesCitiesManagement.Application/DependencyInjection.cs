using CountriesCitiesManagement.Application.Behaviors;
using CountriesCitiesManagement.Application.Interfaces;
using CountriesCitiesManagement.Application.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CountriesCitiesManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, ServiceLifetime.Transient);
        services.AddMediatR(configuration =>configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped<ICountryService, CountryService>();
        services.AddScoped<ICityService, CityService>();

        return services;
    }
}
