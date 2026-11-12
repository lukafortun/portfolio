using Portfolio.Api.DTOs;
using Portfolio.Api.Entities;

namespace Portfolio.Api.Mappings;

public static class ProjectMappings
{
    public static ProjectDto ToDto(this Project p) =>
        new(p.Id, p.Title, p.Description, p.RepoUrl, p.LiveUrl, p.CreatedAt);

    public static Project ToEntity(this ProjectRequest r) => new()
    {
        Title = r.Title,
        Description = r.Description,
        RepoUrl = r.RepoUrl,
        LiveUrl = r.LiveUrl,
    };

    public static void ApplyTo(this ProjectRequest r, Project p)
    {
        p.Title = r.Title;
        p.Description = r.Description;
        p.RepoUrl = r.RepoUrl;
        p.LiveUrl = r.LiveUrl;
    }
}
