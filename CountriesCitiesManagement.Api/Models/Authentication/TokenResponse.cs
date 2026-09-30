namespace CountriesCitiesManagement.Api.Models.Authentication;

public sealed record TokenResponse(string AccessToken,string TokenType,DateTime ExpiresAtUtc);
