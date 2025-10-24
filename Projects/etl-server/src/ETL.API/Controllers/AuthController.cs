using ETL.API.Infrastructure;
using ETL.Application.Auth;
using ETL.Application.Auth.DTOs;
using ETL.Application.Common.Options;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ETL.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly AuthOptions _authOptions;

    public AuthController(IMediator mediator, IOptions<AuthOptions> options)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _authOptions = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    [HttpGet("login")]
    public IActionResult Login([FromQuery] string? redirectPath)
    {
        var authUrl = $"{_authOptions.Authority}/protocol/openid-connect/auth";
        var clientId = _authOptions.ClientId;
        var redirectUri = $"{_authOptions.RedirectUri}/{redirectPath?.TrimStart('/')}";

        var finalUrl = $"{authUrl}?" +
                       $"client_id={Uri.EscapeDataString(clientId)}&" +
                       $"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
                       $"response_type=code&" +
                       $"scope=openid%20profile%20email";

        return Ok(new { redirectUrl = finalUrl });
    }

    [HttpPost("login-callback")]
    public async Task<IActionResult> Callback([FromBody] LoginCallbackCommand request)
    {
        var result = await _mediator.Send(request);

        if (result.IsFailure)
            return this.ToActionResult(result.Error);

        var tokens = result.Value;

        var accessCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddSeconds(tokens.AccessExpiresIn)
        };

        var refreshCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddSeconds(tokens.RefreshExpiresIn),
        };

        Response.Cookies.Append("access_token", tokens.AccessToken ?? string.Empty, accessCookieOptions);
        Response.Cookies.Append("refresh_token", tokens.RefreshToken ?? string.Empty, refreshCookieOptions);

        return Ok(new { message = "Authentication successful" });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(CancellationToken ct)
    {
        var refreshToken = Request.Cookies["refresh_token"];

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");
            return Unauthorized(new { message = "No refresh token present." });
        }

        var result = await _mediator.Send(new RefreshTokenCommand(refreshToken), ct);

        if (result.IsFailure)
        {
            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");

            return this.ToActionResult(result.Error);
        }

        var tokens = result.Value!;

        var accessCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddSeconds(tokens.AccessExpiresIn)
        };

        var refreshCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddSeconds(tokens.RefreshExpiresIn),
        };

        Response.Cookies.Append("access_token", tokens.AccessToken!, accessCookieOptions);
        Response.Cookies.Append("refresh_token", tokens.RefreshToken!, refreshCookieOptions);


        return Ok(new { message = "Token refreshed." });
    }

    [Authorize]
    [HttpPost("change-pass")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
    {
        var result = await _mediator.Send(new ChangePasswordCommand(request, User));

        if (result.IsFailure)
            return this.ToActionResult(result.Error);

        return Ok(new { message = "Password changed successfully." });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var accessToken = Request.Cookies["access_token"];
        var refreshToken = Request.Cookies["refresh_token"];

        var result = await _mediator.Send(new LogoutCommand(accessToken, refreshToken));

        if (result.IsFailure)
            return this.ToActionResult(result.Error);

        Response.Cookies.Delete("access_token");
        Response.Cookies.Delete("refresh_token");

        Response.Headers.CacheControl = "no-cache, no-store";
        Response.Headers.Pragma = "no-cache";
        Response.Headers.Expires = "0";

        return Ok(new { message = "Logout successful" });
    }
}
