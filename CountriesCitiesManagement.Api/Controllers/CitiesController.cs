using CountriesCitiesManagement.Api.Models;
using CountriesCitiesManagement.Application.Features.Cities;
using CountriesCitiesManagement.Application.Models.Cities;
using CountriesCitiesManagement.Application.Models.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CountriesCitiesManagement.Api.Controllers;

[ApiController]
[Route("api/cities")]
public sealed class CitiesController : ControllerBase
{
    private readonly ISender sender;

    public CitiesController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CityDto>>> Create(
        [FromBody] CreateCityRequest request)
    {
        var city = await sender.Send(new CreateCityCommand(request.Name, request.CountryId));
        return Ok(ApiResponse<CityDto>.Succeeded(city));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CityDto>>> GetById(
        int id)
    {
        var city = await sender.Send(new GetCityByIdQuery(id));
        return Ok(ApiResponse<CityDto>.Succeeded(city));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResponse<CityDto>>>> GetPaged(
        [FromQuery] PaginationQuery query)
    {
        var cities = await sender.Send(new GetCitiesQuery(query.PageNumber, query.PageSize, query.Search));
        return Ok(ApiResponse<PagedResponse<CityDto>>.Succeeded(cities));
    }

    [HttpGet("/api/countries/{countryId}/cities")]
    public async Task<ActionResult<ApiResponse<PagedResponse<CityDto>>>> GetByCountry(
        int countryId,
        [FromQuery] PaginationQuery query)
    {
        var cities = await sender.Send(
            new GetCitiesByCountryQuery(
                countryId,
                query.PageNumber,
                query.PageSize,
                query.Search));
        return Ok(ApiResponse<PagedResponse<CityDto>>.Succeeded(cities));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<CityDto>>> Update(
        int id,
        [FromBody] UpdateCityRequest request)
    {
        var city = await sender.Send(new UpdateCityCommand(id, request.Name, request.CountryId));
        return Ok(ApiResponse<CityDto>.Succeeded(city));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(int id)
    {
        await sender.Send(new DeleteCityCommand(id));
        return Ok(ApiResponse<object?>.Succeeded(null));
    }
}
