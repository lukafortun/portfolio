using System.IdentityModel.Tokens.Jwt;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Portfolio.Api.Services;

namespace Portfolio.Api.Extensions;

public static class AuthSetup
{
    public const string AdminPolicy = "Admin";
    public const string AdminGroup = "portfolio-admins";

    /// <summary>Cookie-based session, sign-in through Authentik (OIDC), and the "Admin" policy.</summary>
    public static IServiceCollection AddAdminAuth(this IServiceCollection services, IWebHostEnvironment env, IConfiguration config)
    {
        services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.Name = "portfolio.auth";
                options.Cookie.HttpOnly = true;
                // Lax, not Strict: the cookie must survive the redirect back from Authentik.
                // Lax still blocks cross-site POST/PUT/DELETE (CSRF).
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = env.IsDevelopment()
                    ? CookieSecurePolicy.SameAsRequest
                    : CookieSecurePolicy.Always;
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.SlidingExpiration = true;

                // API endpoints return 401/403 instead of redirecting to a login page.
                options.Events.OnRedirectToLogin = ctx =>
                {
                    ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = ctx =>
                {
                    ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                };
            }).AddOpenIdConnect(options =>
            {
                options.Authority = config["Authentik:Authority"];
                options.ClientId = config["Authentik:ClientId"];
                options.ClientSecret = config["Authentik:ClientSecret"];
                options.CallbackPath = "/api/auth/callback";
                options.SignedOutCallbackPath = "/api/auth/signout-callback";
                options.Scope.Add("email");
                options.TokenValidationParameters.RoleClaimType = "groups";
                // Also keep the groups returned by the userinfo endpoint (one claim per group).
                options.ClaimActions.MapJsonKey("groups", "groups");

                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.ResponseType = OpenIdConnectResponseType.Code;

                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;

                options.MapInboundClaims = false;
                options.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
            });

        services.AddAuthorizationBuilder()
            .AddPolicy(AdminPolicy, p => p.RequireRole(AdminGroup));
        return services;
    }
}
