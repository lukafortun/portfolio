using System.Net;
using System.Net.Http.Json;
using Portfolio.Api.DTOs;

namespace Portfolio.Api.Tests.Integration;

public class ProfileTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private static ProfileRequest Valid() => new(
        "Ada Lovelace",
        new() { ["fr"] = "Développeuse", ["en"] = "Developer" },
        new() { ["fr"] = "Bio en français", ["en"] = "Bio in English" },
        new() { ["fr"] = "Londres" },
        [new("github", "https://github.com/ada")]);

    [Fact]
    public async Task Get_is_public_and_returns_the_seeded_profile()
    {
        var profile = await factory.CreateAnonymousClient().GetFromJsonAsync<ProfileDto>("/api/profile");

        Assert.False(string.IsNullOrEmpty(profile!.Name));
        Assert.True(profile.Role.ContainsKey("fr"));
    }

    [Fact]
    public async Task Save_when_anonymous_returns_401()
    {
        var response = await factory.CreateAnonymousClient().PutAsJsonAsync("/api/profile", Valid());

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Save_when_not_admin_returns_403()
    {
        var client = factory.CreateUserClient("visiteur@test.local", "autre-groupe");

        var response = await client.PutAsJsonAsync("/api/profile", Valid());

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Admin_can_save_the_profile()
    {
        var input = Valid() with
        {
            Location = new() { ["fr"] = "  Londres  ", ["en"] = "" },
            Email = " ada@example.com ",
        };

        (await factory.CreateUserClient().PutAsJsonAsync("/api/profile", input)).EnsureSuccessStatusCode();

        var profile = await factory.CreateAnonymousClient().GetFromJsonAsync<ProfileDto>("/api/profile");
        Assert.Equal("Ada Lovelace", profile!.Name);
        Assert.Equal("Bio in English", profile.Bio["en"]);
        // Texts are trimmed and empty translations dropped.
        Assert.Equal("Londres", profile.Location["fr"]);
        Assert.False(profile.Location.ContainsKey("en"));
        Assert.Equal("https://github.com/ada", Assert.Single(profile.Links).Url);
        Assert.Equal("ada@example.com", profile.Email);
    }

    [Fact]
    public async Task Email_is_optional()
    {
        (await factory.CreateUserClient().PutAsJsonAsync("/api/profile", Valid() with { Email = null }))
            .EnsureSuccessStatusCode();

        var profile = await factory.CreateAnonymousClient().GetFromJsonAsync<ProfileDto>("/api/profile");
        Assert.Null(profile!.Email);
    }

    public static TheoryData<string, ProfileRequest> InvalidInputs => new()
    {
        { "missing default-language role", Valid() with { Role = new() { ["en"] = "Developer" } } },
        { "unsupported language", Valid() with { Bio = new() { ["fr"] = "Bio", ["de"] = "Bio" } } },
        { "bio too long", Valid() with { Bio = new() { ["fr"] = new string('a', 2001) } } },
        { "invalid link url", Valid() with { Links = [new("github", "pas-une-url")] } },
        { "empty name", Valid() with { Name = "" } },
        { "invalid email", Valid() with { Email = "pas-un-email" } },
    };

    [Theory]
    [MemberData(nameof(InvalidInputs))]
    public async Task Save_with_invalid_input_returns_400(string _, ProfileRequest input)
    {
        var response = await factory.CreateUserClient().PutAsJsonAsync("/api/profile", input);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
