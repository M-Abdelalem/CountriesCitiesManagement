using CountriesCitiesManagement.Api.Models;
using CountriesCitiesManagement.Application.Features.Countries;
using CountriesCitiesManagement.Application.Models.Common;
using CountriesCitiesManagement.Application.Models.Countries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CountriesCitiesManagement.Api.Controllers;

[ApiController]
[Route("api/countries")]
public sealed class CountriesController : ControllerBase
{
    private readonly ISender sender;

    public CountriesController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CountryDto>>> Create(
        [FromBody] CreateCountryRequest request)
    {
        var country = await sender.Send(
            new CreateCountryCommand(request.Name, request.Code));
        return Ok(ApiResponse<CountryDto>.Succeeded(country));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CountryDto>>> GetById(
        int id)
    {
        var country = await sender.Send(new GetCountryByIdQuery(id));
        return Ok(ApiResponse<CountryDto>.Succeeded(country));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResponse<CountryDto>>>> GetPaged(
        [FromQuery] PaginationQuery query)
    {
        var countries = await sender.Send(
            new GetCountriesQuery(query.PageNumber, query.PageSize, query.Search));
        return Ok(ApiResponse<PagedResponse<CountryDto>>.Succeeded(countries));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<CountryDto>>> Update(
        int id,
        [FromBody] UpdateCountryRequest request)
    {
        var country = await sender.Send(
            new UpdateCountryCommand(id, request.Name, request.Code));
        return Ok(ApiResponse<CountryDto>.Succeeded(country));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(int id)
    {
        await sender.Send(new DeleteCountryCommand(id));
        return Ok(ApiResponse<object?>.Succeeded(null));
    }
}
