using CountriesCitiesManagement.Api.Authentication;
using CountriesCitiesManagement.Api.Models.Authentication;
using MediatR;

namespace CountriesCitiesManagement.Api.Features.Authentication;

public sealed record LoginCommand(
    string Username,
    string Password) : IRequest<TokenResponse?>;

internal sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, TokenResponse?>
{
    private readonly IAuthenticationService authenticationService;

    public LoginCommandHandler(IAuthenticationService authenticationService)
    {
        this.authenticationService = authenticationService;
    }

    public Task<TokenResponse?> Handle(LoginCommand request,CancellationToken _)
        => Task.FromResult(
            authenticationService.Authenticate(
                request.Username,
                request.Password));
}
