using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CountriesCitiesManagement.Api.Models.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace CountriesCitiesManagement.Api.Authentication;

public interface IAuthenticationService
{
    TokenResponse? Authenticate(string username, string password);
}

internal sealed class AuthenticationService : IAuthenticationService
{
    private readonly AuthenticationOptions options;
    private readonly SigningCredentials signingCredentials;

    public AuthenticationService(AuthenticationOptions options)
    {
        this.options = options;
        signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Convert.FromBase64String(options.SigningKey)),SecurityAlgorithms.HmacSha256);
    }

    public TokenResponse? Authenticate(string username, string password)
    {
        if (username != options.Username || password != options.Password)
        {
            return null;
        }

        var issuedAtUtc = DateTime.UtcNow;
        var expiresAtUtc = issuedAtUtc.AddMinutes(options.TokenLifetimeMinutes);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, options.Username),
            new Claim("name", options.Username)
        };

        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            notBefore: issuedAtUtc,
            expires: expiresAtUtc,
            signingCredentials: signingCredentials);

        return new TokenResponse(
            new JwtSecurityTokenHandler().WriteToken(token),
            "Bearer",
            expiresAtUtc);
    }
}
