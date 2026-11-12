using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Portfolio.Api.Tests.Integration;

/// <summary>
/// Fake authentication scheme replacing Authentik in tests. A request is authenticated when it has
/// an X-Test-User header (the email), with the comma-separated groups from X-Test-Groups.
/// </summary>
public class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "Test";
    public const string UserHeader = "X-Test-User";
    public const string GroupsHeader = "X-Test-Groups";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(UserHeader, out var email))
            return Task.FromResult(AuthenticateResult.NoResult());

        var claims = new List<Claim> { new("email", email.ToString()) };
        var groups = Request.Headers[GroupsHeader].ToString();
        claims.AddRange(groups.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(g => new Claim("groups", g.Trim())));

        // Same claim types as the real OIDC setup: name = "name", roles = "groups".
        var identity = new ClaimsIdentity(claims, SchemeName, nameType: "name", roleType: "groups");
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), SchemeName);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
