using CountriesCitiesManagement.Api.Features.Authentication;
using FluentValidation;

namespace CountriesCitiesManagement.Api.Validation;

internal sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(command => command.Username)
            .NotEmpty()
            .WithMessage("Username is required.");

        RuleFor(command => command.Password)
            .NotEmpty()
            .WithMessage("Password is required.");
    }
}
