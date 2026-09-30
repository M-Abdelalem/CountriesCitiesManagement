using CountriesCitiesManagement.Application.Interfaces;
using CountriesCitiesManagement.Application.Models.Cities;
using FluentValidation;

namespace CountriesCitiesManagement.Application.Validation;

public sealed class CreateCityRequestValidator : AbstractValidator<CreateCityRequest>
{
    public CreateCityRequestValidator(ICityRepository cityRepository,ICountryRepository countryRepository)
    {
        RuleFor(request => request.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters.")
            .MustAsync(async (request, name, _) =>
                !await cityRepository.NameExistsAsync(name, request.CountryId, null))
            .When(request => request.CountryId > 0, ApplyConditionTo.CurrentValidator)
            .WithMessage(request => $"A city named '{request.Name}' already exists in country {request.CountryId}.")
            .WithErrorCode(ValidationErrorCodes.Conflict);

        RuleFor(request => request.CountryId)
            .Cascade(CascadeMode.Stop)
            .GreaterThan(0)
            .WithMessage("Country ID must be greater than zero.")
            .MustAsync(async (countryId, _) =>
                await countryRepository.ExistsAsync(countryId))
            .WithMessage(request => $"Country with ID {request.CountryId} was not found.")
            .WithErrorCode(ValidationErrorCodes.NotFound);
    }
}

public sealed class UpdateCityRequestValidator : AbstractValidator<UpdateCityRequest>
{
    public UpdateCityRequestValidator(ICityRepository cityRepository,ICountryRepository countryRepository)
    {
        RuleFor(request => request.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters.")
            .MustAsync(async (request, name, context, _) =>
                !await cityRepository.NameExistsAsync(
                    name,
                    request.CountryId,
                    GetRouteId(context)))
            .When(request => request.CountryId > 0, ApplyConditionTo.CurrentValidator)
            .WithMessage(request => $"A city named '{request.Name}' already exists in country {request.CountryId}.")
            .WithErrorCode(ValidationErrorCodes.Conflict);

        RuleFor(request => request.CountryId)
            .Cascade(CascadeMode.Stop)
            .GreaterThan(0)
            .WithMessage("Country ID must be greater than zero.")
            .MustAsync(async (countryId, _) =>
                await countryRepository.ExistsAsync(countryId))
            .WithMessage(request => $"Country with ID {request.CountryId} was not found.")
            .WithErrorCode(ValidationErrorCodes.NotFound);
    }

    private static int? GetRouteId(ValidationContext<UpdateCityRequest> context)
        => context.RootContextData.TryGetValue("id", out var value) && value is int id
            ? id
            : null;
}
