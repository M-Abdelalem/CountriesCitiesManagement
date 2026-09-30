using CountriesCitiesManagement.Application.Features.Cities;
using CountriesCitiesManagement.Application.Features.Countries;
using CountriesCitiesManagement.Application.Interfaces;
using FluentValidation;

namespace CountriesCitiesManagement.Application.Validation;

public sealed class GetCountriesQueryValidator : AbstractValidator<GetCountriesQuery>
{
    public GetCountriesQueryValidator()
    {
        RuleFor(query => query.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than zero.");
    }
}

public sealed class GetCitiesQueryValidator : AbstractValidator<GetCitiesQuery>
{
    public GetCitiesQueryValidator()
    {
        RuleFor(query => query.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than zero.");
    }
}

public sealed class GetCitiesByCountryQueryValidator
    : AbstractValidator<GetCitiesByCountryQuery>
{
    public GetCitiesByCountryQueryValidator(ICountryRepository countryRepository)
    {
        RuleFor(query => query.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than zero.");

        RuleFor(query => query.CountryId)
            .Cascade(CascadeMode.Stop)
            .GreaterThan(0)
            .WithMessage("Country ID must be greater than zero.")
            .MustAsync(async (countryId, _) =>
                await countryRepository.ExistsAsync(countryId))
            .WithMessage(query => $"Country with ID {query.CountryId} was not found.")
            .WithErrorCode(ValidationErrorCodes.NotFound);
    }
}
