using CountriesCitiesManagement.Application.Interfaces;
using CountriesCitiesManagement.Application.Models;
using CountriesCitiesManagement.Domain.Entities;
using CountriesCitiesManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CountriesCitiesManagement.Infrastructure.Repositories;

internal sealed class CityRepository : ICityRepository
{
    private readonly CountriesCitiesDbContext dbContext;

    public CityRepository(CountriesCitiesDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<City?> GetByIdAsync(int id)
    {
        var city = await dbContext.Cities
            .SingleOrDefaultAsync(c => c.Id == id);

        return city;
    }

    public async Task<PagedData<City>> GetPagedAsync(int pageNumber,int pageSize,string? search,int? countryId)
    {
        var query = dbContext.Cities.AsNoTracking();

        if (countryId.HasValue)
        {
            query = query.Where(city => city.CountryId == countryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(city => city.Name.Contains(search));
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(city => city.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedData<City>(items, totalCount);
    }

    public async Task<bool> NameExistsAsync(string name,int countryId,int? excludedId)
    {
        var query = dbContext.Cities
            .Where(city => city.CountryId == countryId)
            .Where(city => city.Name == name);

        if (excludedId.HasValue)
        {
            query = query.Where(city => city.Id != excludedId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task AddAsync(City city)
    {
        await dbContext.Cities.AddAsync(city);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(City city)
    {
        dbContext.Cities.Update(city);
        await dbContext.SaveChangesAsync();
    }

    public async Task RemoveAsync(City city)
    {
        dbContext.Cities.Remove(city);
        await dbContext.SaveChangesAsync();
    }
}
