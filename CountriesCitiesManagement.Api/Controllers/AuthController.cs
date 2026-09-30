using CountriesCitiesManagement.Api.Authentication;
using CountriesCitiesManagement.Api.Models;
using CountriesCitiesManagement.Api.Models.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CountriesCitiesManagement.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthenticationService authenticationService;

    public AuthController(IAuthenticationService authenticationService)
    {
        this.authenticationService = authenticationService;
    }

    [AllowAnonymous]
    [HttpPost("token")]
    public ActionResult<ApiResponse<TokenResponse>> CreateToken([FromBody] LoginRequest request)
    {
        var token = authenticationService.Authenticate(
            request.Username,
            request.Password);

        if (token is null)
        {
            return Unauthorized(
                ApiResponse<object?>.Failed(
                    StatusCodes.Status401Unauthorized,
                    "Invalid username or password."));
        }

        return Ok(ApiResponse<TokenResponse>.Succeeded(token));
    }
}
