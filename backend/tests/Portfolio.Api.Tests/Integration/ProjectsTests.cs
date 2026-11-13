using System.Net;
using System.Net.Http.Json;
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
        { "duplicate language", ValidInput("e") with { Translations = [new("fr", "T", "S", null), new("fr", "T2", "S2", null)] } },
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
}
