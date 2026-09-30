using CountriesCitiesManagement.Application.Features.Countries;
using CountriesCitiesManagement.Application.Interfaces;
using FluentValidation;

namespace CountriesCitiesManagement.Application.Validation;

public sealed class CreateCountryRequestValidator : AbstractValidator<CreateCountryCommand>
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

public sealed class UpdateCountryRequestValidator : AbstractValidator<UpdateCountryCommand>
{
    public UpdateCountryRequestValidator(ICountryRepository countryRepository)
    {
        RuleFor(request => request.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters.")
            .MustAsync(async (request, name, _) =>
                !await countryRepository.NameExistsAsync(name, request.Id))
            .WithMessage(request => $"A country named '{request.Name}' already exists.")
            .WithErrorCode(ValidationErrorCodes.Conflict);

        RuleFor(request => request.Code)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Code is required.")
            .Matches("^[A-Za-z]{3}$")
            .WithMessage("Code must contain exactly three letters.")
            .MustAsync(async (request, code, _) =>
                !await countryRepository.CodeExistsAsync(code, request.Id))
            .WithMessage(request => $"A country with code '{request.Code}' already exists.")
            .WithErrorCode(ValidationErrorCodes.Conflict);
    }
}
