using CountriesCitiesManagement.Application.Interfaces;
using CountriesCitiesManagement.Application.Models.Countries;
using MediatR;

namespace CountriesCitiesManagement.Application.Features.Countries;

public sealed record CreateCountryCommand(string Name, string Code) : IRequest<CountryDto>;

public sealed record UpdateCountryCommand(int Id, string Name, string Code) : IRequest<CountryDto>;

public sealed record DeleteCountryCommand(int Id) : IRequest;

internal sealed class CreateCountryCommandHandler
    : IRequestHandler<CreateCountryCommand, CountryDto>
{
    private readonly ICountryService countryService;

    public CreateCountryCommandHandler(ICountryService countryService)
    {
        this.countryService = countryService;
    }

    public Task<CountryDto> Handle(
        CreateCountryCommand request,
        CancellationToken _)
        => countryService.CreateAsync(
            new CreateCountryRequest
            {
                Name = request.Name,
                Code = request.Code
            });
}

internal sealed class UpdateCountryCommandHandler
    : IRequestHandler<UpdateCountryCommand, CountryDto>
{
    private readonly ICountryService countryService;

    public UpdateCountryCommandHandler(ICountryService countryService)
    {
        this.countryService = countryService;
    }

    public Task<CountryDto> Handle(
        UpdateCountryCommand request,
        CancellationToken _)
        => countryService.UpdateAsync(
            request.Id,
            new UpdateCountryRequest
            {
                Name = request.Name,
                Code = request.Code
            });
}

internal sealed class DeleteCountryCommandHandler
    : IRequestHandler<DeleteCountryCommand>
{
    private readonly ICountryService countryService;

    public DeleteCountryCommandHandler(ICountryService countryService)
    {
        this.countryService = countryService;
    }

    public Task Handle(
        DeleteCountryCommand request,
        CancellationToken _)
        => countryService.DeleteAsync(request.Id);
}
