using CountriesCitiesManagement.Application.Models.Cities;
using CountriesCitiesManagement.Application.Models.Common;
using CountriesCitiesManagement.Application.Exceptions;
using CountriesCitiesManagement.Application.Interfaces;
using CountriesCitiesManagement.Domain.Entities;

namespace CountriesCitiesManagement.Application.Services;

internal sealed class CityService : ICityService
{
    private readonly ICityRepository cityRepository;

    public CityService(ICityRepository cityRepository)
    {
        this.cityRepository = cityRepository;
    }

    public async Task<CityDto> CreateAsync(CreateCityRequest request)
    {
        var city = new City
        {
            Name = request.Name,
            CountryId = request.CountryId
        };

        await cityRepository.AddAsync(city);

        return Map(city);
    }

    public async Task<CityDto> GetByIdAsync(int id)
    {
        var city = await GetCityAsync(id);
        return Map(city);
    }

    public Task<PagedResponse<CityDto>> GetPagedAsync(PaginationQuery query)
        => GetPageAsync(query, null);

    public async Task<PagedResponse<CityDto>> GetByCountryAsync(int countryId,PaginationQuery query)
    {
        return await GetPageAsync(query, countryId);
    }

    public async Task<CityDto> UpdateAsync(int id, UpdateCityRequest request)
    {
        var city = await GetCityAsync(id);

        city.Name = request.Name;
        city.CountryId = request.CountryId;
        await cityRepository.UpdateAsync(city);

        return Map(city);
    }

    public async Task DeleteAsync(int id)
    {
        var city = await GetCityAsync(id);
        await cityRepository.RemoveAsync(city);
    }

    private async Task<PagedResponse<CityDto>> GetPageAsync(PaginationQuery query,int? countryId)
    {
        var page = await cityRepository.GetPagedAsync(
            query.PageNumber,
            query.PageSize,
            query.Search,
            countryId);

        return PagedResponse<CityDto>.Create(
            page.Items.Select(Map).ToList(),
            query.PageNumber,
            query.PageSize,
            page.TotalCount);
    }

    private async Task<City> GetCityAsync(int id)
    {
        return await cityRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"City with ID {id} was not found.");
    }

    private static CityDto Map(City city) => new(city.Id, city.Name, city.CountryId);
}
