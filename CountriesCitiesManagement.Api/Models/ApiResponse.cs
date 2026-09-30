namespace CountriesCitiesManagement.Api.Models;

public sealed record ApiResponse<T>(T? Data,int Code,string? ErrorMessage,bool Success)
{
    public static ApiResponse<T> Succeeded(T? data,int code = StatusCodes.Status200OK)
    {
        return new ApiResponse<T>(data, code, null, true);
    }

    public static ApiResponse<T> Failed(int code, string errorMessage)
    {
        return new ApiResponse<T>(default, code, errorMessage, false);
    }
}
