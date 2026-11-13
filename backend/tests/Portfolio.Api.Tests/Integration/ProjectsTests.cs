using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Api.Data;
using Portfolio.Api.DTOs;

namespace Portfolio.Api.Tests.Integration;

public class ProjectsTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    // Tests in this class share one database: each one uses its own slug.
    private static ProjectRequest ValidInput(string slug) => new(
        slug,
        [
            new("fr", "Portfolio", "Mon site perso", "## Stack\n\n- React\n- ASP.NET Core"),
            new("en", "Portfolio EN", "My personal website", "## Stack"),
        ],
        "https://github.com/moi/portfolio",
        null);

    [Fact]
    public async Task List_is_public()
    {
        var response = await factory.CreateAnonymousClient().GetAsync("/api/projects");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Create_when_anonymous_returns_401()
    {
        var response = await factory.CreateAnonymousClient().PostAsJsonAsync("/api/projects", ValidInput("anonymous"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Create_when_not_admin_returns_403()
    {
        var client = factory.CreateUserClient("visiteur@test.local", "autre-groupe");

        var response = await client.PostAsJsonAsync("/api/projects", ValidInput("not-admin"));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    public static TheoryData<string, ProjectRequest> InvalidInputs => new()
    {
        { "empty title", ValidInput("a") with { Translations = [new("fr", "", "summary", null)] } },
        { "invalid url", ValidInput("b") with { RepoUrl = "pas-une-url" } },
        { "invalid slug", ValidInput("Mon Projet!") },
        { "cover not a url", ValidInput("f") with { CoverImage = "javascript:alert(1)" } },
        { "cover path traversal", ValidInput("g") with { CoverImage = "/api/uploads/../appsettings.json" } },
        { "missing default language", ValidInput("c") with { Translations = [new("en", "Title", "summary", null)] } },
        { "unsupported language", ValidInput("d") with { Translations = [new("fr", "T", "S", null), new("de", "T", "S", null)] } },
        { "empty tag", ValidInput("h") with { Tags = ["react", " "] } },
        { "tag too long", ValidInput("i") with { Tags = [new string('a', 51)] } },
        { "too many tags", ValidInput("j") with { Tags = Enumerable.Range(0, 16).Select(i => $"t{i}").ToList() } },
        { "duplicate language",ValidInput("e") with { Translations = [new("fr", "T", "S", null), new("fr", "T2", "S2", null)] } },
    };

    [Theory]
    [MemberData(nameof(InvalidInputs))]
    public async Task Create_with_invalid_input_returns_400(string _, ProjectRequest input)
    {
        var response = await factory.CreateUserClient().PostAsJsonAsync("/api/projects", input);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_with_a_slug_already_used_returns_409()
    {
        var admin = factory.CreateUserClient();
        (await admin.PostAsJsonAsync("/api/projects", ValidInput("duplicate"))).EnsureSuccessStatusCode();

        var response = await admin.PostAsJsonAsync("/api/projects", ValidInput("duplicate"));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Admin_can_create_update_and_delete_a_project()
    {
        var admin = factory.CreateUserClient();
        var anonymous = factory.CreateAnonymousClient();

        // Create
        var created = await admin.PostAsJsonAsync("/api/projects", ValidInput("crud"));
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var project = await created.Content.ReadFromJsonAsync<ProjectDto>();
        Assert.NotNull(project);

        // Listed publicly, in the requested language
        var list = await anonymous.GetFromJsonAsync<List<ProjectListItemDto>>("/api/projects?lang=en");
        var item = list!.Single(p => p.Id == project.Id);
        Assert.Equal("Portfolio EN", item.Title);
        Assert.Equal(["fr", "en"], item.Languages);

        // Project page, by slug, with its Markdown content
        var page = await anonymous.GetFromJsonAsync<ProjectDto>("/api/projects/by-slug/crud?lang=fr");
        Assert.Equal("Portfolio", page!.Title);
        Assert.StartsWith("## Stack", page.Content);

        // Update: new slug, English translation dropped → English requests fall back to French
        var updated = await admin.PutAsJsonAsync($"/api/projects/{project.Id}",
            ValidInput("crud-renamed") with { Translations = [new("fr", "Nouveau titre", "Résumé", "Texte")] });
        updated.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NotFound, (await anonymous.GetAsync("/api/projects/by-slug/crud")).StatusCode);
        var fetched = await anonymous.GetFromJsonAsync<ProjectDto>("/api/projects/by-slug/crud-renamed?lang=en");
        Assert.Equal("fr", fetched!.Language);
        Assert.Equal("Nouveau titre", fetched.Title);

        // Keeping its own slug on update is not a conflict; the cover image is saved
        (await admin.PutAsJsonAsync($"/api/projects/{project.Id}",
            ValidInput("crud-renamed") with { CoverImage = "/api/uploads/abc.webp" })).EnsureSuccessStatusCode();
        var withCover = await anonymous.GetFromJsonAsync<List<ProjectListItemDto>>("/api/projects");
        Assert.Equal("/api/uploads/abc.webp", withCover!.Single(p => p.Id == project.Id).CoverImage);

        // Delete (translations are deleted in cascade)
        Assert.Equal(HttpStatusCode.NoContent, (await admin.DeleteAsync($"/api/projects/{project.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await anonymous.GetAsync($"/api/projects/{project.Id}")).StatusCode);
    }

    [Fact]
    public async Task Tags_are_normalized()
    {
        var admin = factory.CreateUserClient();
        var input = ValidInput("tags") with { Tags = ["#React", " react ", "Spring  Boot", "DOCKER"] };

        var created = await admin.PostAsJsonAsync("/api/projects", input);
        created.EnsureSuccessStatusCode();

        // Lowercase, no "#", spaces → dashes, duplicates removed, sorted; returned by the list and by the page.
        string[] expected = ["docker", "react", "spring-boot"];
        Assert.Equal(expected, (await created.Content.ReadFromJsonAsync<ProjectDto>())!.Tags);
        var list = await factory.CreateAnonymousClient().GetFromJsonAsync<List<ProjectListItemDto>>("/api/projects");
        Assert.Equal(expected, list!.Single(p => p.Slug == "tags").Tags);
    }

    [Fact]
    public async Task Tags_are_shared_between_projects_and_deleted_when_unused()
    {
        var admin = factory.CreateUserClient();
        var first = await (await admin.PostAsJsonAsync("/api/projects", ValidInput("shared-1") with { Tags = ["shared", "only-one"] }))
            .Content.ReadFromJsonAsync<ProjectDto>();
        var second = await (await admin.PostAsJsonAsync("/api/projects", ValidInput("shared-2") with { Tags = ["shared"] }))
            .Content.ReadFromJsonAsync<ProjectDto>();

        // One "shared" row, linked to both projects.
        Assert.Equal(1, await CountTagsAsync("shared"));

        // "only-one" loses its last project: deleted. "shared" is still used by the second project.
        (await admin.PutAsJsonAsync($"/api/projects/{first!.Id}", ValidInput("shared-1"))).EnsureSuccessStatusCode();
        Assert.Equal(0, await CountTagsAsync("only-one"));
        Assert.Equal(1, await CountTagsAsync("shared"));

        // Deleting the last project that uses "shared" deletes it too.
        (await admin.DeleteAsync($"/api/projects/{second!.Id}")).EnsureSuccessStatusCode();
        Assert.Equal(0, await CountTagsAsync("shared"));
    }

    [Fact]
    public async Task Tags_endpoint_lists_tags_with_their_project_count()
    {
        var admin = factory.CreateUserClient();
        (await admin.PostAsJsonAsync("/api/projects", ValidInput("count-1") with { Tags = ["counted", "solo"] })).EnsureSuccessStatusCode();
        (await admin.PostAsJsonAsync("/api/projects", ValidInput("count-2") with { Tags = ["counted"] })).EnsureSuccessStatusCode();

        // Public, like the project list.
        var tags = await factory.CreateAnonymousClient().GetFromJsonAsync<List<TagDto>>("/api/tags");

        Assert.Equal(2, tags!.Single(t => t.Name == "counted").ProjectCount);
        Assert.Equal(1, tags.Single(t => t.Name == "solo").ProjectCount);
        // Most used first.
        Assert.True(tags.FindIndex(t => t.Name == "counted") < tags.FindIndex(t => t.Name == "solo"));
    }

    /// <summary>Reads the Tags table directly: the API never exposes tag rows.</summary>
    private async Task<int> CountTagsAsync(string name)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await db.Tags.CountAsync(t => t.Name == name);
    }
}
