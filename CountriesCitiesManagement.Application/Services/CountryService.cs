using CountriesCitiesManagement.Application.Models.Common;
using CountriesCitiesManagement.Application.Models.Countries;
using CountriesCitiesManagement.Application.Exceptions;
using CountriesCitiesManagement.Application.Interfaces;
using CountriesCitiesManagement.Domain.Entities;

namespace CountriesCitiesManagement.Application.Services;

internal sealed class CountryService : ICountryService
{
    private readonly ICountryRepository countryRepository;

    public CountryService(ICountryRepository countryRepository)
    {
        this.countryRepository = countryRepository;
    }

    public async Task<CountryDto> CreateAsync(CreateCountryRequest request)
    {

        var country = new Country
        {
            Name = request.Name,
            Code = request.Code
        };

        await countryRepository.AddAsync(country);

        return Map(country);
    }

    public async Task<CountryDto> GetByIdAsync(int id)
    {
        var country = await GetCountryAsync(id);
        return Map(country);
    }

    public async Task<PagedResponse<CountryDto>> GetPagedAsync(PaginationQuery query)
    {
        var page = await countryRepository.GetPagedAsync(
            query.PageNumber,
            query.PageSize,
            query.Search);

        return PagedResponse<CountryDto>.Create(
            page.Items.Select(Map).ToList(),
            query.PageNumber,
            query.PageSize,
            page.TotalCount);
    }

    public async Task<CountryDto> UpdateAsync(int id, UpdateCountryRequest request)
    {
        var country = await GetCountryAsync(id);

        country.Name = request.Name;
        country.Code = request.Code;
        await countryRepository.UpdateAsync(country);

        return Map(country);
    }

    public async Task DeleteAsync(int id)
    {
        var country = await GetCountryAsync(id);

        if (await countryRepository.HasCitiesAsync(id))
        {
            throw new ConflictException("The country cannot be deleted while it has cities.");
        }

        await countryRepository.RemoveAsync(country);
    }

    private async Task<Country> GetCountryAsync(int id)
    {
        return await countryRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Country with ID {id} was not found.");
    }

    private static CountryDto Map(Country country) => new(country.Id, country.Name, country.Code);
}
