using CountriesCitiesManagement.Application.Models.Common;
using CountriesCitiesManagement.Application.Models.Countries;

namespace CountriesCitiesManagement.Application.Interfaces;

public interface ICountryService
{
    Task<CountryDto> CreateAsync(CreateCountryRequest request);
    Task<CountryDto> GetByIdAsync(int id);
    Task<PagedResponse<CountryDto>> GetPagedAsync(PaginationQuery query);
    Task<CountryDto> UpdateAsync(int id, UpdateCountryRequest request);
    Task DeleteAsync(int id);
}
