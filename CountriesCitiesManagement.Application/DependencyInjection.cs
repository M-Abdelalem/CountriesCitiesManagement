using CountriesCitiesManagement.Application.Interfaces;
using CountriesCitiesManagement.Application.Services;
using CountriesCitiesManagement.Application.Validation;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CountriesCitiesManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateCountryRequestValidator>(ServiceLifetime.Transient);
        services.AddScoped<ICountryService, CountryService>();
        services.AddScoped<ICityService, CityService>();

        return services;
    }
}
