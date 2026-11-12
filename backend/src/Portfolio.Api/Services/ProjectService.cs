using Portfolio.Api.DTOs;
using Portfolio.Api.Mappings;
using Portfolio.Api.Repositories;

namespace Portfolio.Api.Services;

public class ProjectService(IProjectRepository repository) : IProjectService
{
    public async Task<IReadOnlyList<ProjectDto>> GetAllAsync(CancellationToken ct = default)
    {
        var projects = await repository.GetAllAsync(ct);
        return projects.Select(p => p.ToDto()).ToList();
    }

    public async Task<ProjectDto?> GetByIdAsync(int id, CancellationToken ct = default) =>
        (await repository.GetByIdAsync(id, ct))?.ToDto();

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
