using CountriesCitiesManagement.Application.Interfaces;
using CountriesCitiesManagement.Application.Models.Countries;
using FluentValidation;

namespace CountriesCitiesManagement.Application.Validation;

public sealed class CreateCountryRequestValidator : AbstractValidator<CreateCountryRequest>
{
    public CreateCountryRequestValidator(ICountryRepository countryRepository)
    {
        RuleFor(request => request.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters.")
            .MustAsync(async (name, _) =>
                !await countryRepository.NameExistsAsync(name, null))
            .WithMessage(request => $"A country named '{request.Name}' already exists.")
            .WithErrorCode(ValidationErrorCodes.Conflict);

        RuleFor(request => request.Code)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Code is required.")
            .Matches("^[A-Za-z]{3}$")
            .WithMessage("Code must contain exactly three letters.")
            .MustAsync(async (code, _) =>
                !await countryRepository.CodeExistsAsync(code, null))
            .WithMessage(request => $"A country with code '{request.Code}' already exists.")
            .WithErrorCode(ValidationErrorCodes.Conflict);
    }
}

public sealed class UpdateCountryRequestValidator : AbstractValidator<UpdateCountryRequest>
{
    public UpdateCountryRequestValidator(ICountryRepository countryRepository)
    {
        RuleFor(request => request.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters.")
            .MustAsync(async (request, name, context, _) =>
                !await countryRepository.NameExistsAsync(name, GetRouteId(context)))
            .WithMessage(request => $"A country named '{request.Name}' already exists.")
            .WithErrorCode(ValidationErrorCodes.Conflict);

        RuleFor(request => request.Code)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Code is required.")
            .Matches("^[A-Za-z]{3}$")
            .WithMessage("Code must contain exactly three letters.")
            .MustAsync(async (request, code, context, _) =>
                !await countryRepository.CodeExistsAsync(code, GetRouteId(context)))
            .WithMessage(request => $"A country with code '{request.Code}' already exists.")
            .WithErrorCode(ValidationErrorCodes.Conflict);
    }

    private static int? GetRouteId(ValidationContext<UpdateCountryRequest> context)
        => context.RootContextData.TryGetValue("id", out var value) && value is int id
            ? id
            : null;
}
