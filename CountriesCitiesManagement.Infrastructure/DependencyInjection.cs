using CountriesCitiesManagement.Application.Interfaces;
using CountriesCitiesManagement.Infrastructure.Persistence;
using CountriesCitiesManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CountriesCitiesManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,string connectionString)
    {
        services.AddDbContext<CountriesCitiesDbContext>(options =>options.UseSqlServer(connectionString));

        services.AddScoped<ICountryRepository, CountryRepository>();
        services.AddScoped<ICityRepository, CityRepository>();

        return services;
    }
}
