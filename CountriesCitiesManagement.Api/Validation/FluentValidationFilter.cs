using CountriesCitiesManagement.Api.Models;
using CountriesCitiesManagement.Application.Validation;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CountriesCitiesManagement.Api.Validation;

internal sealed class FluentValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context,ActionExecutionDelegate next)
    {
        var failures = new List<ValidationFailure>();

        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null)
            {
                continue;
            }

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            if (context.HttpContext.RequestServices.GetService(validatorType) is not IValidator validator)
            {
                continue;
            }

            var validationContext = new ValidationContext<object>(argument);
            foreach (var actionArgument in context.ActionArguments)
            {
                if (actionArgument.Value is not null)
                {
                    validationContext.RootContextData[actionArgument.Key] = actionArgument.Value;
                }
            }

            var result = await validator.ValidateAsync(
                validationContext,
                context.HttpContext.RequestAborted);

            failures.AddRange(result.Errors);
        }

        if (failures.Count == 0)
        {
            await next();
            return;
        }

        var errorMessage = string.Join(
            "; ",
            failures
                .Select(failure => failure.ErrorMessage)
                .Distinct());

        var hasBadRequestFailure = failures.Any(failure =>
            failure.ErrorCode != ValidationErrorCodes.NotFound &&
            failure.ErrorCode != ValidationErrorCodes.Conflict);

        var statusCode = hasBadRequestFailure
            ? StatusCodes.Status400BadRequest
            : failures.Any(failure => failure.ErrorCode == ValidationErrorCodes.NotFound)
                ? StatusCodes.Status404NotFound
                : StatusCodes.Status409Conflict;

        context.Result = new ObjectResult(
            ApiResponse<object?>.Failed(statusCode, errorMessage))
        {
            StatusCode = statusCode
        };
    }
}
