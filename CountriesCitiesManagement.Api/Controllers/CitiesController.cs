using CountriesCitiesManagement.Api.Models;
using CountriesCitiesManagement.Application.Models.Cities;
using CountriesCitiesManagement.Application.Models.Common;
using CountriesCitiesManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CountriesCitiesManagement.Api.Controllers;

[ApiController]
[Route("api/cities")]
public sealed class CitiesController : ControllerBase
{
    private readonly ICityService cityService;

    public CitiesController(ICityService cityService)
    {
        this.cityService = cityService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CityDto>>> Create(
        [FromBody] CreateCityRequest request)
    {
        var city = await cityService.CreateAsync(request);
        return Ok(ApiResponse<CityDto>.Succeeded(city));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CityDto>>> GetById(int id)
    {
        var city = await cityService.GetByIdAsync(id);
        return Ok(ApiResponse<CityDto>.Succeeded(city));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResponse<CityDto>>>> GetPaged([FromQuery] PaginationQuery query)
    {
        var cities = await cityService.GetPagedAsync(query);
        return Ok(ApiResponse<PagedResponse<CityDto>>.Succeeded(cities));
    }

    [HttpGet("/api/countries/{countryId}/cities")]
    public async Task<ActionResult<ApiResponse<PagedResponse<CityDto>>>> GetByCountry(int countryId,[FromQuery] PaginationQuery query)
    {
        var cities = await cityService.GetByCountryAsync(countryId, query);
        return Ok(ApiResponse<PagedResponse<CityDto>>.Succeeded(cities));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<CityDto>>> Update(
        int id,
        [FromBody] UpdateCityRequest request)
    {
        var city = await cityService.UpdateAsync(id, request);
        return Ok(ApiResponse<CityDto>.Succeeded(city));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(int id)
    {
        await cityService.DeleteAsync(id);
        return Ok(ApiResponse<object?>.Succeeded(null));
    }
}
