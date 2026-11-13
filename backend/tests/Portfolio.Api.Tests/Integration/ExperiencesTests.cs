using System.Net;
using System.Net.Http.Json;
using Portfolio.Api.DTOs;

namespace Portfolio.Api.Tests.Integration;

public class ExperiencesTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private static ExperienceRequest Valid(string start = "2020-01", string? end = "2020-12") => new(
        start,
        end,
        "ACME",
        new() { ["fr"] = "Développeur", ["en"] = "Developer" },
        null,
        new() { ["fr"] = "Description" },
        ["C#"]);

    [Fact]
    public async Task List_is_public_with_ongoing_entries_first()
    {
        var list = await factory.CreateAnonymousClient().GetFromJsonAsync<List<ExperienceDto>>("/api/experiences");

        Assert.NotEmpty(list!); // seeded
        Assert.Null(list![0].End);
    }

    [Fact]
    public async Task Create_when_not_admin_returns_403()
    {
        var client = factory.CreateUserClient("visiteur@test.local", "autre-groupe");

        var response = await client.PostAsJsonAsync("/api/experiences", Valid());

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    public static TheoryData<string, ExperienceRequest> InvalidInputs => new()
    {
        { "bad start format", Valid(start: "2020-13") },
        { "bad end format", Valid(end: "décembre") },
        { "end before start", Valid(start: "2021-01", end: "2020-12") },
        { "missing role", Valid() with { Role = new() { ["en"] = "Developer" } } },
        { "empty company", Valid() with { Company = "" } },
    };

    [Theory]
    [MemberData(nameof(InvalidInputs))]
    public async Task Create_with_invalid_input_returns_400(string _, ExperienceRequest input)
    {
        var response = await factory.CreateUserClient().PostAsJsonAsync("/api/experiences", input);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Admin_can_create_update_and_delete_an_entry()
    {
        var admin = factory.CreateUserClient();
        var anonymous = factory.CreateAnonymousClient();

        // Create an old entry: listed last
        var created = await admin.PostAsJsonAsync("/api/experiences", Valid("1990-01", "1990-06"));
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var entry = await created.Content.ReadFromJsonAsync<ExperienceDto>();
        var list = await anonymous.GetFromJsonAsync<List<ExperienceDto>>("/api/experiences");
        Assert.Equal(entry!.Id, list![^1].Id);
        Assert.Empty(list[^1].Location); // null → empty

        // Update to ongoing: listed among the first
        (await admin.PutAsJsonAsync($"/api/experiences/{entry.Id}", Valid("2099-01", null))).EnsureSuccessStatusCode();
        list = await anonymous.GetFromJsonAsync<List<ExperienceDto>>("/api/experiences");
        Assert.Equal(entry.Id, list![0].Id);

        // Delete
        Assert.Equal(HttpStatusCode.NoContent, (await admin.DeleteAsync($"/api/experiences/{entry.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await admin.PutAsJsonAsync($"/api/experiences/{entry.Id}", Valid())).StatusCode);
    }
}
