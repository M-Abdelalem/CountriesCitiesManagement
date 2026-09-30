using CountriesCitiesManagement.Application.Models;
using CountriesCitiesManagement.Domain.Entities;

namespace CountriesCitiesManagement.Application.Interfaces;

public interface ICityRepository
{
    Task<City?> GetByIdAsync(int id);
    Task<PagedData<City>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search,
        int? countryId);
    Task<bool> NameExistsAsync(string name, int countryId, int? excludedId);
    Task AddAsync(City city);
    Task UpdateAsync(City city);
    Task RemoveAsync(City city);
}
