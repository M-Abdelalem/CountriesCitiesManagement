using CountriesCitiesManagement.Application.Interfaces;
using CountriesCitiesManagement.Application.Models.Common;
using CountriesCitiesManagement.Application.Models.Countries;
using MediatR;

namespace CountriesCitiesManagement.Application.Features.Countries;

public sealed record GetCountryByIdQuery(int Id) : IRequest<CountryDto>;

public sealed record GetCountriesQuery(
    int PageNumber,
    int PageSize,
    string? Search) : IRequest<PagedResponse<CountryDto>>;

internal sealed class GetCountryByIdQueryHandler
    : IRequestHandler<GetCountryByIdQuery, CountryDto>
{
    private readonly ICountryService countryService;

    public GetCountryByIdQueryHandler(ICountryService countryService)
    {
        this.countryService = countryService;
    }

    public Task<CountryDto> Handle(
        GetCountryByIdQuery request,
        CancellationToken _)
        => countryService.GetByIdAsync(request.Id);
}

internal sealed class GetCountriesQueryHandler
    : IRequestHandler<GetCountriesQuery, PagedResponse<CountryDto>>
{
    private readonly ICountryService countryService;

    public GetCountriesQueryHandler(ICountryService countryService)
    {
        this.countryService = countryService;
    }

    public Task<PagedResponse<CountryDto>> Handle(
        GetCountriesQuery request,
        CancellationToken _)
        => countryService.GetPagedAsync(
            new PaginationQuery
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                Search = request.Search
            });
}
