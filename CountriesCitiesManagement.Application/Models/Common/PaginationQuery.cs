namespace CountriesCitiesManagement.Application.Models.Common;

public sealed class PaginationQuery
{
    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 10;

    public string? Search { get; init; }
}
