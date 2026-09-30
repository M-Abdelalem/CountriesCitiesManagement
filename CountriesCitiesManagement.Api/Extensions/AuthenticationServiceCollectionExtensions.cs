using CountriesCitiesManagement.Api.Authentication;
using CountriesCitiesManagement.Api.Models;
using CountriesCitiesManagement.Api.Models.Authentication;
using CountriesCitiesManagement.Api.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace CountriesCitiesManagement.Api.Extensions;

internal static class AuthenticationServiceCollectionExtensions
{
    public static IServiceCollection AddApiAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var authenticationOptions = configuration
            .GetSection(AuthenticationOptions.SectionName)
            .Get<AuthenticationOptions>()
            ?? throw new InvalidOperationException(
                $"Configuration section '{AuthenticationOptions.SectionName}' is required.");

        var signingKeyBytes = ValidateAndGetSigningKey(authenticationOptions);

        services.AddSingleton(authenticationOptions);
        services.AddSingleton<IAuthenticationService, AuthenticationService>();
        services.AddTransient<IValidator<LoginRequest>, LoginRequestValidator>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = authenticationOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = authenticationOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = "name"
                };

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        if (context.Response.HasStarted)
                        {
                            return;
                        }

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsJsonAsync(
                            ApiResponse<object?>.Failed(
                                StatusCodes.Status401Unauthorized,
                                "Authentication is required."),
                            context.HttpContext.RequestAborted);
                    },
                    OnForbidden = async context =>
                    {
                        if (context.Response.HasStarted)
                        {
                            return;
                        }

                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsJsonAsync(
                            ApiResponse<object?>.Failed(
                                StatusCodes.Status403Forbidden,
                                "Access is forbidden."),
                            context.HttpContext.RequestAborted);
                    }
                };
            });

        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        return services;
    }

    public static IServiceCollection AddApiSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition(
                JwtBearerDefaults.AuthenticationScheme,
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Enter the JWT bearer token."
                });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(
                    JwtBearerDefaults.AuthenticationScheme,
                    document)] = []
            });
        });

        return services;
    }

    private static byte[] ValidateAndGetSigningKey(AuthenticationOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Username) ||
            string.IsNullOrWhiteSpace(options.Password) ||
            string.IsNullOrWhiteSpace(options.Issuer) ||
            string.IsNullOrWhiteSpace(options.Audience) ||
            string.IsNullOrWhiteSpace(options.SigningKey) ||
            options.TokenLifetimeMinutes <= 0)
        {
            throw new InvalidOperationException(
                $"Configuration section '{AuthenticationOptions.SectionName}' is incomplete.");
        }

        try
        {
            var signingKeyBytes = Convert.FromBase64String(options.SigningKey);
            if (signingKeyBytes.Length < 32)
            {
                throw new InvalidOperationException("Authentication signing key must contain at least 32 bytes.");
            }

            return signingKeyBytes;
        }
        catch (FormatException exception)
        {
            throw new InvalidOperationException(
                "Authentication signing key must be valid Base64.",
                exception);
        }
    }
}
