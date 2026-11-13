using System.Net.Http.Json;
using Portfolio.Api.DTOs;

namespace Portfolio.Api.Tests.Integration;

/// <summary>Tags shared by projects and experience entries.</summary>
public class TagsTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private static ProjectRequest Project(string slug, params string[] tags) =>
        new(slug, [new("fr", "Projet", "Résumé", null)], null, null, Tags: [.. tags]);

    private static ExperienceRequest Experience(params string[] tags) =>
        new("2020-01", "2020-12", "ACME", new() { ["fr"] = "Développeur" }, null, null, [.. tags]);

    private async Task<TagDto?> GetTagAsync(string name) =>
        (await factory.CreateAnonymousClient().GetFromJsonAsync<List<TagDto>>("/api/tags"))!.SingleOrDefault(t => t.Name == name);

    [Fact]
    public async Task A_tag_is_shared_by_projects_and_experience_entries()
    {
        var admin = factory.CreateUserClient();
        var project = await (await admin.PostAsJsonAsync("/api/projects", Project("with-shared", "#Shared-Tag")))
            .Content.ReadFromJsonAsync<ProjectDto>();
        var entry = await (await admin.PostAsJsonAsync("/api/experiences", Experience("shared tag")))
            .Content.ReadFromJsonAsync<ExperienceDto>();

        // Normalized the same way on both sides, so it is one tag used twice.
        Assert.Equal(["shared-tag"], entry!.Tags);
        Assert.Equal(new TagDto("shared-tag", 1, 1), await GetTagAsync("shared-tag"));

        // Still used by the experience entry: kept.
        (await admin.DeleteAsync($"/api/projects/{project!.Id}")).EnsureSuccessStatusCode();
        Assert.Equal(new TagDto("shared-tag", 0, 1), await GetTagAsync("shared-tag"));

        // Nothing uses it anymore: deleted.
        (await admin.DeleteAsync($"/api/experiences/{entry.Id}")).EnsureSuccessStatusCode();
        Assert.Null(await GetTagAsync("shared-tag"));
    }

    [Fact]
    public async Task Experience_tags_are_updated()
    {
        var admin = factory.CreateUserClient();
        var entry = await (await admin.PostAsJsonAsync("/api/experiences", Experience("one", "two")))
            .Content.ReadFromJsonAsync<ExperienceDto>();

        var updated = await (await admin.PutAsJsonAsync($"/api/experiences/{entry!.Id}", Experience("two", "three")))
            .Content.ReadFromJsonAsync<ExperienceDto>();

        Assert.Equal(["three", "two"], updated!.Tags);
        Assert.Null(await GetTagAsync("one"));
    }
}
