using CountriesCitiesManagement.Application.Interfaces;
using CountriesCitiesManagement.Application.Models.Cities;
using CountriesCitiesManagement.Application.Models.Common;
using MediatR;

namespace CountriesCitiesManagement.Application.Features.Cities;

public sealed record GetCityByIdQuery(int Id) : IRequest<CityDto>;

public sealed record GetCitiesQuery(
    int PageNumber,
    int PageSize,
    string? Search) : IRequest<PagedResponse<CityDto>>;

public sealed record GetCitiesByCountryQuery(
    int CountryId,
    int PageNumber,
    int PageSize,
    string? Search) : IRequest<PagedResponse<CityDto>>;

internal sealed class GetCityByIdQueryHandler
    : IRequestHandler<GetCityByIdQuery, CityDto>
{
    private readonly ICityService cityService;

    public GetCityByIdQueryHandler(ICityService cityService)
    {
        this.cityService = cityService;
    }

    public Task<CityDto> Handle(
        GetCityByIdQuery request,
        CancellationToken _)
        => cityService.GetByIdAsync(request.Id);
}

internal sealed class GetCitiesQueryHandler
    : IRequestHandler<GetCitiesQuery, PagedResponse<CityDto>>
{
    private readonly ICityService cityService;

    public GetCitiesQueryHandler(ICityService cityService)
    {
        this.cityService = cityService;
    }

    public Task<PagedResponse<CityDto>> Handle(
        GetCitiesQuery request,
        CancellationToken _)
        => cityService.GetPagedAsync(
            new PaginationQuery
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                Search = request.Search
            });
}

internal sealed class GetCitiesByCountryQueryHandler
    : IRequestHandler<GetCitiesByCountryQuery, PagedResponse<CityDto>>
{
    private readonly ICityService cityService;

    public GetCitiesByCountryQueryHandler(ICityService cityService)
    {
        this.cityService = cityService;
    }

    public Task<PagedResponse<CityDto>> Handle(
        GetCitiesByCountryQuery request,
        CancellationToken _)
        => cityService.GetByCountryAsync(
            request.CountryId,
            new PaginationQuery
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                Search = request.Search
            });
}
