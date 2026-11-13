using System.Net;
using System.Net.Http.Json;
using Portfolio.Api.DTOs;

namespace Portfolio.Api.Tests.Integration;

public class SkillsTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private static SkillGroupRequest Valid(string name = "mobile", int sortOrder = 5) => new(
        name,
        sortOrder,
        new() { ["fr"] = "Applications mobiles", ["en"] = "Mobile apps" },
        ["Kotlin", " Swift ", "Kotlin"]);

    [Fact]
    public async Task List_is_public_and_sorted()
    {
        var groups = await factory.CreateAnonymousClient().GetFromJsonAsync<List<SkillGroupDto>>("/api/skills");

        Assert.NotEmpty(groups!); // seeded
        Assert.Equal(groups!.OrderBy(g => g.SortOrder).Select(g => g.Id), groups.Select(g => g.Id));
    }

    [Fact]
    public async Task Create_when_anonymous_returns_401()
    {
        var response = await factory.CreateAnonymousClient().PostAsJsonAsync("/api/skills", Valid());

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    public static TheoryData<string, SkillGroupRequest> InvalidInputs => new()
    {
        { "empty name", Valid() with { Name = "" } },
        { "empty skill", Valid() with { Skills = ["React", " "] } },
        { "skill too long", Valid() with { Skills = [new string('a', 51)] } },
        { "negative order", Valid() with { SortOrder = -1 } },
    };

    [Theory]
    [MemberData(nameof(InvalidInputs))]
    public async Task Create_with_invalid_input_returns_400(string _, SkillGroupRequest input)
    {
        var response = await factory.CreateUserClient().PostAsJsonAsync("/api/skills", input);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Admin_can_create_update_and_delete_a_group()
    {
        var admin = factory.CreateUserClient();
        var anonymous = factory.CreateAnonymousClient();

        // Create: skills are trimmed and deduplicated; sortOrder 5 puts it first
        var created = await admin.PostAsJsonAsync("/api/skills", Valid());
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var group = await created.Content.ReadFromJsonAsync<SkillGroupDto>();
        Assert.Equal(["Kotlin", "Swift"], group!.Skills);
        var list = await anonymous.GetFromJsonAsync<List<SkillGroupDto>>("/api/skills");
        Assert.Equal(group.Id, list![0].Id);

        // Update
        (await admin.PutAsJsonAsync($"/api/skills/{group.Id}", Valid("mobile-dev", 999))).EnsureSuccessStatusCode();
        list = await anonymous.GetFromJsonAsync<List<SkillGroupDto>>("/api/skills");
        Assert.Equal("mobile-dev", list![^1].Name);

        // Delete
        Assert.Equal(HttpStatusCode.NoContent, (await admin.DeleteAsync($"/api/skills/{group.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await admin.DeleteAsync($"/api/skills/{group.Id}")).StatusCode);
    }
}
