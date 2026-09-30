using CountriesCitiesManagement.Application.Models;
using CountriesCitiesManagement.Domain.Entities;

namespace CountriesCitiesManagement.Application.Interfaces;

public interface ICountryRepository
{
    Task<Country?> GetByIdAsync(int id);
    Task<PagedData<Country>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search);
    Task<bool> ExistsAsync(int id);
    Task<bool> NameExistsAsync(string name, int? excludedId);
    Task<bool> CodeExistsAsync(string code, int? excludedId);
    Task<bool> HasCitiesAsync(int id);
    Task AddAsync(Country country);
    Task UpdateAsync(Country country);
    Task RemoveAsync(Country country);
}
