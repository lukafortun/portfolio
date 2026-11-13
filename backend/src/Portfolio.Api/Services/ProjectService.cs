using Portfolio.Api.DTOs;
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
        await repository.AddAsync(project, ct);
        await repository.SaveChangesAsync(ct);
        return project.ToDto();
    }

    public async Task<ProjectDto?> UpdateAsync(int id, ProjectRequest request, CancellationToken ct = default)
    {
        var project = await repository.GetByIdAsync(id, ct);
        if (project is null) return null;

        request.ApplyTo(project);
        await repository.SaveChangesAsync(ct);
        return project.ToDto();
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var project = await repository.GetByIdAsync(id, ct);
        if (project is null) return false;

        repository.Remove(project);
        await repository.SaveChangesAsync(ct);
        return true;
    }
}
