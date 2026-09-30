namespace CountriesCitiesManagement.Api.Authentication;

internal sealed class AuthenticationOptions
{
    internal const string SectionName = "Authentication";

    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string SigningKey { get; init; } = string.Empty;
    public int TokenLifetimeMinutes { get; init; }
}
