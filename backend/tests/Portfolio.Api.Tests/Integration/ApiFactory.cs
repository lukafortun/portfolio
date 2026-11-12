using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Portfolio.Api.Data;
using Portfolio.Api.Extensions;

namespace Portfolio.Api.Tests.Integration;

/// <summary>
/// Hosts the API in memory with SQLite instead of PostgreSQL and <see cref="TestAuthHandler"/>
/// instead of Authentik. OIDC metadata is hard-coded so no network call is ever made.
/// </summary>
public class ApiFactory : WebApplicationFactory<Program>
{
    public const string FakeAuthentik = "https://authentik.test/application/o/portfolio/";

    // An in-memory SQLite database only lives while its connection stays open.
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _connection.Open();

        builder.UseEnvironment("Testing");
        builder.UseSetting("Authentik:Authority", FakeAuthentik);
        builder.UseSetting("Authentik:ClientId", "test-client");
        builder.UseSetting("Authentik:ClientSecret", "test-secret");

        // Runs after Program.cs, so it can override what Program.cs registered.
        builder.ConfigureTestServices(services =>
        {
            var toRemove = services.Where(d =>
                d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                d.ServiceType == typeof(IDbContextOptionsConfiguration<AppDbContext>)).ToList();
            foreach (var d in toRemove) services.Remove(d);
            services.AddDbContext<AppDbContext>(o => o.UseSqlite(_connection));

            services.AddAuthentication(TestAuthHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });

            services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, o =>
                o.Configuration = new OpenIdConnectConfiguration
                {
                    Issuer = FakeAuthentik,
                    AuthorizationEndpoint = "https://authentik.test/application/o/authorize/",
                    TokenEndpoint = "https://authentik.test/application/o/token/",
                });
        });
    }

    /// <summary>Anonymous client. Does not follow redirects so tests can assert on them.</summary>
    public HttpClient CreateAnonymousClient() =>
        CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false,
        });

    /// <summary>Client signed in as an Authentik user, in the admin group unless other groups are given.</summary>
    public HttpClient CreateUserClient(string email = "admin@test.local", params string[] groups)
    {
        var client = CreateAnonymousClient();
        client.DefaultRequestHeaders.Add(TestAuthHandler.UserHeader, email);
        client.DefaultRequestHeaders.Add(TestAuthHandler.GroupsHeader,
            string.Join(',', groups.Length > 0 ? groups : [AuthSetup.AdminGroup]));
        return client;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing) _connection.Dispose();
    }
}
