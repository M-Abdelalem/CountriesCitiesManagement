using CountriesCitiesManagement.Application.Models.Authentication;

namespace CountriesCitiesManagement.Application.Interfaces;

public interface IAuthenticationService
{
    TokenResponse? Authenticate(string username, string password);
}
