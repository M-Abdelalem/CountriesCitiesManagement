namespace CountriesCitiesManagement.Application.Models.Cities;

public sealed class UpdateCityRequest
{
    public string Name { get; init; } = string.Empty;

    public int CountryId { get; init; }
}
