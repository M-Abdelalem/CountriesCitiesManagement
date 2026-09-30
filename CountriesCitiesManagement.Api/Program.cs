using CountriesCitiesManagement.Api.ExceptionHandling;
using CountriesCitiesManagement.Api.Extensions;
using CountriesCitiesManagement.Api.Models;
using CountriesCitiesManagement.Application;
using CountriesCitiesManagement.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using ApiServiceCollectionExtensions = CountriesCitiesManagement.Api.Extensions.ServiceCollectionExtensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers(options =>
    {
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errorMessage = string.Join(
                "; ",
                context.ModelState.Values
                    .SelectMany(entry => entry.Errors)
                    .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                        ? "The supplied value is invalid."
                        : error.ErrorMessage)
                    .Distinct());

            return new BadRequestObjectResult(
                ApiResponse<object?>.Failed(
                    StatusCodes.Status400BadRequest,
                    errorMessage));
        };
    });
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiSwagger();
builder.Services.AddApplication();
builder.Services.AddApiCors(builder.Configuration);
builder.Services.AddApiAuthentication(builder.Configuration);
builder.Services.AddApiRateLimiting(builder.Configuration);
builder.Services.AddApiSerilog(builder.Configuration);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' is required.");
builder.Services.AddInfrastructure(connectionString);
var app = builder.Build();
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["Referrer-Policy"] = "no-referrer";
    context.Response.Headers["Permissions-Policy"] =
        "camera=(), microphone=(), geolocation=()";

    await next();
});

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseSerilogRequestLogging();
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors(ApiServiceCollectionExtensions.FrontendCorsPolicy);
app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers().RequireRateLimiting(ApiServiceCollectionExtensions.PerEndpointRateLimitPolicy);

app.Run();
