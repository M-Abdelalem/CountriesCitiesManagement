namespace CountriesCitiesManagement.Application.Models.Cities;

public sealed class CreateCityRequest
{
    public string Name { get; init; } = string.Empty;

    public int CountryId { get; init; }
}
