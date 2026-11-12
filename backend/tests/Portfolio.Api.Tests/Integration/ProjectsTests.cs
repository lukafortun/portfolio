using System.Net;
using System.Net.Http.Json;
using Portfolio.Api.DTOs;

namespace Portfolio.Api.Tests.Integration;

public class ProjectsTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private static readonly ProjectRequest ValidInput =
        new("Portfolio", "Mon site perso", "https://github.com/moi/portfolio", null);

    [Fact]
    public async Task List_is_public()
    {
        var response = await factory.CreateAnonymousClient().GetAsync("/api/projects");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Create_when_anonymous_returns_401()
    {
        var response = await factory.CreateAnonymousClient().PostAsJsonAsync("/api/projects", ValidInput);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Create_when_not_admin_returns_403()
    {
        var client = factory.CreateUserClient("visiteur@test.local", "autre-groupe");

        var response = await client.PostAsJsonAsync("/api/projects", ValidInput);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Create_with_invalid_input_returns_400()
    {
        var response = await factory.CreateUserClient().PostAsJsonAsync("/api/projects",
            new ProjectRequest("", "desc", "pas-une-url", null));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Admin_can_create_update_and_delete_a_project()
    {
        var admin = factory.CreateUserClient();
        var anonymous = factory.CreateAnonymousClient();

        // Create
        var created = await admin.PostAsJsonAsync("/api/projects", ValidInput);
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var project = await created.Content.ReadFromJsonAsync<ProjectDto>();
        Assert.NotNull(project);

        // Publicly visible
        var list = await anonymous.GetFromJsonAsync<List<ProjectDto>>("/api/projects");
        Assert.Contains(list!, p => p.Id == project.Id);

        // Update
        var updated = await admin.PutAsJsonAsync($"/api/projects/{project.Id}", ValidInput with { Title = "Nouveau titre" });
        updated.EnsureSuccessStatusCode();
        var fetched = await anonymous.GetFromJsonAsync<ProjectDto>($"/api/projects/{project.Id}");
        Assert.Equal("Nouveau titre", fetched!.Title);

        // Delete
        Assert.Equal(HttpStatusCode.NoContent, (await admin.DeleteAsync($"/api/projects/{project.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await anonymous.GetAsync($"/api/projects/{project.Id}")).StatusCode);
    }
}
