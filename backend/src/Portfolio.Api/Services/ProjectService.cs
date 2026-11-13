using Portfolio.Api.DTOs;
using Portfolio.Api.Entities;
using Portfolio.Api.Mappings;
using Portfolio.Api.Repositories;

namespace Portfolio.Api.Services;

public class ProjectService(IProjectRepository repository) : IProjectService
{
    public async Task<IReadOnlyList<ProjectListItemDto>> GetAllAsync(string? language = null, CancellationToken ct = default)
    {
        var projects = await repository.GetAllAsync(ct);
        return projects.Select(p => p.ToListItem(language)).ToList();
    }

    public async Task<ProjectDto?> GetByIdAsync(int id, string? language = null, CancellationToken ct = default) =>
        (await repository.GetByIdAsync(id, ct))?.ToDto(language);

    public async Task<ProjectDto?> GetBySlugAsync(string slug, string? language = null, CancellationToken ct = default) =>
        (await repository.GetBySlugAsync(slug, ct))?.ToDto(language);

    public Task<bool> IsSlugTakenAsync(string slug, int? exceptId = null, CancellationToken ct = default) =>
        repository.SlugExistsAsync(slug, exceptId, ct);

    public async Task<ProjectDto> CreateAsync(ProjectRequest request, CancellationToken ct = default)
    {
        var project = request.ToEntity();
        await SetTagsAsync(project, request.Tags, ct);
        await repository.AddAsync(project, ct);
        await repository.SaveChangesAsync(ct);
        return project.ToDto();
    }

    public async Task<ProjectDto?> UpdateAsync(int id, ProjectRequest request, CancellationToken ct = default)
    {
        var project = await repository.GetByIdAsync(id, ct);
        if (project is null) return null;

        request.ApplyTo(project);
        await SetTagsAsync(project, request.Tags, ct);
        await repository.SaveChangesAsync(ct);
        // A tag removed from its last project is deleted.
        await repository.RemoveUnusedTagsAsync(ct);
        return project.ToDto();
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var project = await repository.GetByIdAsync(id, ct);
        if (project is null) return false;

        repository.Remove(project);
        await repository.SaveChangesAsync(ct);
        await repository.RemoveUnusedTagsAsync(ct);
        return true;
    }

    public async Task<IReadOnlyList<TagDto>> GetTagsAsync(CancellationToken ct = default) =>
        await repository.GetTagUsageAsync(ct);

    /// <summary>
    /// Links the project to the requested tags: an existing tag is reused (one "react" row shared by
    /// every project), a new one is created; tags missing from the request are unlinked.
    /// </summary>
    private async Task SetTagsAsync(Project project, IEnumerable<string>? requested, CancellationToken ct)
    {
        var names = ProjectMappings.NormalizeTags(requested);

        project.Tags.RemoveAll(tag => !names.Contains(tag.Name));

        var missing = names.Except(project.Tags.Select(tag => tag.Name)).ToList();
        if (missing.Count == 0) return;

        var existing = await repository.GetTagsAsync(missing, ct);
        foreach (var name in missing)
            project.Tags.Add(existing.Find(tag => tag.Name == name) ?? new Tag { Name = name });
    }
}
