using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Portfolio.Api.DTOs;
using Portfolio.Api.Extensions;
using Portfolio.Api.Services;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController() : ControllerBase
{
    [HttpGet("login")]
    public IActionResult Login(string? returnUrl = "/admin") =>
        Challenge(new AuthenticationProperties
        {
            RedirectUri = Url.IsLocalUrl(returnUrl) ? returnUrl : "/admin",
        }, OpenIdConnectDefaults.AuthenticationScheme);

    // POST, not GET: otherwise any website could log the user out with a plain link.
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return NoContent();
    }

    [HttpGet("me")]
    [Authorize(Policy = AuthSetup.AdminPolicy)]
    public ActionResult<MeResponse> Me() => Ok(new MeResponse(User.FindFirstValue(JwtRegisteredClaimNames.Email)!));
}
