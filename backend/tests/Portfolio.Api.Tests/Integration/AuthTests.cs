using System.Net;
using System.Net.Http.Json;
using Portfolio.Api.DTOs;

namespace Portfolio.Api.Tests.Integration;

public class AuthTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    [Fact]
    public async Task Me_when_anonymous_returns_401()
    {
        var response = await factory.CreateAnonymousClient().GetAsync("/api/auth/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Me_when_not_in_admin_group_returns_403()
    {
        var client = factory.CreateUserClient("visiteur@test.local", "autre-groupe");

        var response = await client.GetAsync("/api/auth/me");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Me_when_admin_returns_email_from_email_claim()
    {
        var me = await factory.CreateUserClient("moi@test.local").GetFromJsonAsync<MeResponse>("/api/auth/me");

        Assert.Equal("moi@test.local", me!.Email);
    }

    [Fact]
    public async Task Login_redirects_to_authentik_with_callback_and_scopes()
    {
        var response = await factory.CreateAnonymousClient().GetAsync("/api/auth/login?returnUrl=/admin");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        var location = response.Headers.Location!.ToString();
        Assert.StartsWith("https://authentik.test/application/o/authorize/", location);
        Assert.Contains("client_id=test-client", location);
        Assert.Contains(Uri.EscapeDataString("https://localhost/api/auth/callback"), location);
        Assert.Contains("response_type=code", location);
        Assert.Contains("email", location);           // email scope
        Assert.Contains("code_challenge=", location);  // PKCE
    }

    [Fact]
    public async Task Logout_is_post_only()
    {
        var client = factory.CreateAnonymousClient();

        Assert.Equal(HttpStatusCode.NoContent, (await client.PostAsync("/api/auth/logout", null)).StatusCode);
        Assert.Equal(HttpStatusCode.MethodNotAllowed, (await client.GetAsync("/api/auth/logout")).StatusCode);
    }
}
