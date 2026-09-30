using CountriesCitiesManagement.Application.Models.Cities;
using CountriesCitiesManagement.Application.Models.Common;

namespace CountriesCitiesManagement.Application.Interfaces;

public interface ICityService
{
    Task<CityDto> CreateAsync(CreateCityRequest request);
    Task<CityDto> GetByIdAsync(int id);
    Task<PagedResponse<CityDto>> GetPagedAsync(PaginationQuery query);
    Task<PagedResponse<CityDto>> GetByCountryAsync(
        int countryId,
        PaginationQuery query);
    Task<CityDto> UpdateAsync(int id, UpdateCityRequest request);
    Task DeleteAsync(int id);
}
