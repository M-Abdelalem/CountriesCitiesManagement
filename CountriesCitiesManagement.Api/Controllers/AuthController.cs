using CountriesCitiesManagement.Api.Features.Authentication;
using CountriesCitiesManagement.Api.Models;
using CountriesCitiesManagement.Api.Models.Authentication;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CountriesCitiesManagement.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly ISender sender;

    public AuthController(ISender sender)
    {
        this.sender = sender;
    }

    [AllowAnonymous]
    [HttpPost("token")]
    public async Task<ActionResult<ApiResponse<TokenResponse>>> CreateToken([FromBody] LoginRequest request)
    {
        var token = await sender.Send(new LoginCommand(request.Username, request.Password));

        if (token is null)
        {
            return Unauthorized(ApiResponse<object?>.Failed(StatusCodes.Status401Unauthorized,"Invalid username or password."));
        }

        return Ok(ApiResponse<TokenResponse>.Succeeded(token));
    }
}
