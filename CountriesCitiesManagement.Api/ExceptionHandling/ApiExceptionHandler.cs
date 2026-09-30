using CountriesCitiesManagement.Api.Models;
using CountriesCitiesManagement.Application.Exceptions;
using CountriesCitiesManagement.Application.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace CountriesCitiesManagement.Api.ExceptionHandling;

internal sealed class ApiExceptionHandler : IExceptionHandler
{
    private readonly ILogger<ApiExceptionHandler> logger;

    public ApiExceptionHandler(ILogger<ApiExceptionHandler> logger)
    {
        this.logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken _)
    {
        var (statusCode, errorMessage) = exception switch
        {
            ValidationException ex => GetValidationError(ex),
            NotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            ConflictException => (StatusCodes.Status409Conflict, exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "The server was unable to process the request.")
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "An unhandled exception occurred while processing the request.");
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(
            ApiResponse<object?>.Failed(statusCode, errorMessage));

        return true;
    }

    private static (int StatusCode, string ErrorMessage) GetValidationError(
        ValidationException exception)
    {
        var failures = exception.Errors.ToList();
        var errorMessage = string.Join(
            "; ",
            failures
                .Select(failure => failure.ErrorMessage)
                .Distinct());

        var hasBadRequestFailure = failures.Count == 0 || failures.Any(failure =>
            failure.ErrorCode != ValidationErrorCodes.NotFound &&
            failure.ErrorCode != ValidationErrorCodes.Conflict);

        var statusCode = hasBadRequestFailure
            ? StatusCodes.Status400BadRequest
            : failures.Any(failure => failure.ErrorCode == ValidationErrorCodes.NotFound)
                ? StatusCodes.Status404NotFound
                : StatusCodes.Status409Conflict;

        return (statusCode, errorMessage);
    }
}
