using CountriesCitiesManagement.Application.Interfaces;
using CountriesCitiesManagement.Application.Models.Cities;
using MediatR;

namespace CountriesCitiesManagement.Application.Features.Cities;

public sealed record CreateCityCommand(string Name, int CountryId) : IRequest<CityDto>;

public sealed record UpdateCityCommand(int Id, string Name, int CountryId) : IRequest<CityDto>;

public sealed record DeleteCityCommand(int Id) : IRequest;

internal sealed class CreateCityCommandHandler
    : IRequestHandler<CreateCityCommand, CityDto>
{
    private readonly ICityService cityService;

    public CreateCityCommandHandler(ICityService cityService)
    {
        this.cityService = cityService;
    }

    public Task<CityDto> Handle(
        CreateCityCommand request,
        CancellationToken _)
        => cityService.CreateAsync(
            new CreateCityRequest
            {
                Name = request.Name,
                CountryId = request.CountryId
            });
}

internal sealed class UpdateCityCommandHandler
    : IRequestHandler<UpdateCityCommand, CityDto>
{
    private readonly ICityService cityService;

    public UpdateCityCommandHandler(ICityService cityService)
    {
        this.cityService = cityService;
    }

    public Task<CityDto> Handle(
        UpdateCityCommand request,
        CancellationToken _)
        => cityService.UpdateAsync(
            request.Id,
            new UpdateCityRequest
            {
                Name = request.Name,
                CountryId = request.CountryId
            });
}

internal sealed class DeleteCityCommandHandler
    : IRequestHandler<DeleteCityCommand>
{
    private readonly ICityService cityService;

    public DeleteCityCommandHandler(ICityService cityService)
    {
        this.cityService = cityService;
    }

    public Task Handle(
        DeleteCityCommand request,
        CancellationToken _)
        => cityService.DeleteAsync(request.Id);
}
