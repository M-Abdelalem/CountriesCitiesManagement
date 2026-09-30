namespace CountriesCitiesManagement.Application.Models.Authentication;

public sealed record TokenResponse(
    string AccessToken,
    string TokenType,
    DateTime ExpiresAtUtc);
