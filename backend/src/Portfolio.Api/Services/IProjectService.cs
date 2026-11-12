using Portfolio.Api.DTOs;

namespace Portfolio.Api.Services;

public interface IProjectService
{
    Task<IReadOnlyList<ProjectDto>> GetAllAsync(CancellationToken ct = default);

    /// <returns>null if the project does not exist.</returns>
    Task<ProjectDto?> GetByIdAsync(int id, CancellationToken ct = default);

    Task<ProjectDto> CreateAsync(ProjectRequest request, CancellationToken ct = default);

    /// <returns>null if the project does not exist.</returns>
    Task<ProjectDto?> UpdateAsync(int id, ProjectRequest request, CancellationToken ct = default);

    /// <returns>false if the project does not exist.</returns>
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
