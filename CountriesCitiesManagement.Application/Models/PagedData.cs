namespace CountriesCitiesManagement.Application.Models;

public sealed record PagedData<T>(IReadOnlyList<T> Items, int TotalCount);
