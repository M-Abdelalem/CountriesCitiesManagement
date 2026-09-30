using CountriesCitiesManagement.Api.Models;
using CountriesCitiesManagement.Application.Models.Common;
using CountriesCitiesManagement.Application.Models.Countries;
using CountriesCitiesManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CountriesCitiesManagement.Api.Controllers;

[ApiController]
[Route("api/countries")]
public sealed class CountriesController : ControllerBase
{
    private readonly ICountryService countryService;

    public CountriesController(ICountryService countryService)
    {
        this.countryService = countryService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CountryDto>>> Create(
        [FromBody] CreateCountryRequest request)
    {
        var country = await countryService.CreateAsync(request);
        return Ok(ApiResponse<CountryDto>.Succeeded(country));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CountryDto>>> GetById(int id)
    {
        var country = await countryService.GetByIdAsync(id);
        return Ok(ApiResponse<CountryDto>.Succeeded(country));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResponse<CountryDto>>>> GetPaged(
        [FromQuery] PaginationQuery query)
    {
        var countries = await countryService.GetPagedAsync(query);
        return Ok(ApiResponse<PagedResponse<CountryDto>>.Succeeded(countries));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<CountryDto>>> Update(
        int id,
        [FromBody] UpdateCountryRequest request)
    {
        var country = await countryService.UpdateAsync(id, request);
        return Ok(ApiResponse<CountryDto>.Succeeded(country));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(int id)
    {
        await countryService.DeleteAsync(id);
        return Ok(ApiResponse<object?>.Succeeded(null));
    }
}
