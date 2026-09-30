namespace CountriesCitiesManagement.Application.Models.Countries;

public sealed class CreateCountryRequest
{
    public string Name { get; init; } = string.Empty;

    public string Code { get; init; } = string.Empty;
}
