using CountriesCitiesManagement.Api.Models.Authentication;
using FluentValidation;

namespace CountriesCitiesManagement.Api.Validation;

internal sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(request => request.Username)
            .NotEmpty()
            .WithMessage("Username is required.");

        RuleFor(request => request.Password)
            .NotEmpty()
            .WithMessage("Password is required.");
    }
}
