using System.Globalization;
using System.Security.Claims;
using System.Threading.RateLimiting;
using CountriesCitiesManagement.Api.Models;
using Serilog;

namespace CountriesCitiesManagement.Api.Extensions;

internal static class ServiceCollectionExtensions
{
    internal const string FrontendCorsPolicy = "frontend";
    internal const string PerEndpointRateLimitPolicy = "per-endpoint";

    public static IServiceCollection AddApiCors(this IServiceCollection services,IConfiguration configuration)
    {
        var allowedOrigins = configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? [];

        services.AddCors(options =>
        {
            options.AddPolicy(FrontendCorsPolicy, policy =>
            {
                if (allowedOrigins.Length > 0)
                {
                    policy
                        .WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                }
            });
        });

        return services;
    }

    public static IServiceCollection AddApiRateLimiting(this IServiceCollection services,IConfiguration configuration)
    {
        var permitLimit = configuration.GetValue<int?>("RateLimiting:PermitLimit") ?? 100;
        var windowSeconds = configuration.GetValue<int?>("RateLimiting:WindowSeconds") ?? 60;

        services.AddRateLimiter(options =>
        {
            options.AddPolicy(PerEndpointRateLimitPolicy, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    GetRateLimitPartitionKey(httpContext),
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = permitLimit,
                        Window = TimeSpan.FromSeconds(windowSeconds),
                        QueueLimit = 0,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        AutoReplenishment = true
                    }));

            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = async (context, cancellationToken) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers["Retry-After"] =
                        Math.Ceiling(retryAfter.TotalSeconds)
                            .ToString(CultureInfo.InvariantCulture);
                }

                await context.HttpContext.Response.WriteAsJsonAsync(
                    ApiResponse<object?>.Failed(
                        StatusCodes.Status429TooManyRequests,
                        "Too many requests. Please try again later."),
                    cancellationToken);
            };
        });

        return services;
    }

    public static IServiceCollection AddApiSerilog(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddSerilog((serviceProvider, loggerConfiguration) => loggerConfiguration
            .ReadFrom.Configuration(configuration)
            .ReadFrom.Services(serviceProvider)
            .Enrich.FromLogContext());

        return services;
    }

    private static string GetRateLimitPartitionKey(HttpContext httpContext)
    {
        if (httpContext.User.Identity?.IsAuthenticated == true)
        {
            var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? httpContext.User.FindFirstValue("sub");

            if (!string.IsNullOrWhiteSpace(userId))
            {
                return GetEndpointPartitionKey(httpContext, $"user:{userId}");
            }
        }

        var remoteIpAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return GetEndpointPartitionKey(httpContext, $"ip:{remoteIpAddress}");
    }

    private static string GetEndpointPartitionKey(HttpContext httpContext, string clientKey)
    {
        var routePattern = (httpContext.GetEndpoint() as RouteEndpoint)?.RoutePattern.RawText
            ?? "unknown";

        return $"{clientKey}|method:{httpContext.Request.Method}|route:{routePattern}";
    }
}
