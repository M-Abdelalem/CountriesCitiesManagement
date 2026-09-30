using CountriesCitiesManagement.Application.Interfaces;
using CountriesCitiesManagement.Application.Models;
using CountriesCitiesManagement.Domain.Entities;
using CountriesCitiesManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CountriesCitiesManagement.Infrastructure.Repositories;

internal sealed class CountryRepository : ICountryRepository
{
    private readonly CountriesCitiesDbContext dbContext;

    public CountryRepository(CountriesCitiesDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Task<Country?> GetByIdAsync(int id)
        => dbContext.Countries.SingleOrDefaultAsync(country => country.Id == id);

    public async Task<PagedData<Country>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search)
    {
        var query = dbContext.Countries.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(country =>
                country.Name.Contains(search) || country.Code.Contains(search));
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(country => country.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedData<Country>(items, totalCount);
    }

    public Task<bool> ExistsAsync(int id)
        => dbContext.Countries.AnyAsync(country => country.Id == id);

    public Task<bool> NameExistsAsync(
        string name,
        int? excludedId)
        => dbContext.Countries.AnyAsync(
            country => country.Name == name && (!excludedId.HasValue || country.Id != excludedId.Value));

    public Task<bool> CodeExistsAsync(
        string code,
        int? excludedId)
        => dbContext.Countries.AnyAsync(
            country => country.Code == code && (!excludedId.HasValue || country.Id != excludedId.Value));

    public Task<bool> HasCitiesAsync(int id)
        => dbContext.Cities.AnyAsync(city => city.CountryId == id);

    public async Task AddAsync(Country country)
    {
        await dbContext.Countries.AddAsync(country);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Country country)
    {
        dbContext.Countries.Update(country);
        await dbContext.SaveChangesAsync();
    }

    public async Task RemoveAsync(Country country)
    {
        dbContext.Countries.Remove(country);
        await dbContext.SaveChangesAsync();
    }
}
