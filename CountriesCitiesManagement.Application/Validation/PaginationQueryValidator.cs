using CountriesCitiesManagement.Application.Interfaces;
using CountriesCitiesManagement.Application.Models.Common;
using FluentValidation;
using FluentValidation.Results;

namespace CountriesCitiesManagement.Application.Validation;

public sealed class PaginationQueryValidator : AbstractValidator<PaginationQuery>
{
    public PaginationQueryValidator(ICountryRepository countryRepository)
    {
        RuleFor(query => query.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than zero.");

        //RuleFor(query => query.PageSize)
        //    .InclusiveBetween(1, 100)
        //    .WithMessage("Page size must be between 1 and 100.");

        RuleFor(query => query)
            .CustomAsync(async (_, context, _) =>
            {
                if (!context.RootContextData.TryGetValue("countryId", out var value) ||
                    value is not int countryId)
                {
                    return;
                }

                if (countryId <= 0)
                {
                    context.AddFailure(
                        new ValidationFailure(
                            "CountryId",
                            "Country ID must be greater than zero."));
                    return;
                }

                if (!await countryRepository.ExistsAsync(countryId))
                {
                    context.AddFailure(
                        new ValidationFailure(
                            "CountryId",
                            $"Country with ID {countryId} was not found.")
                        {
                            ErrorCode = ValidationErrorCodes.NotFound
                        });
                }
            });
    }
}
