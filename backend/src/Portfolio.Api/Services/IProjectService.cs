using Portfolio.Api.DTOs;

namespace Portfolio.Api.Services;

public interface IProjectService
{
    /// <param name="language">Language of the returned texts (fallback: default language).</param>
    Task<IReadOnlyList<ProjectListItemDto>> GetAllAsync(string? language = null, CancellationToken ct = default);

    /// <returns>null if the project does not exist.</returns>
    Task<ProjectDto?> GetByIdAsync(int id, string? language = null, CancellationToken ct = default);

    /// <returns>null if the project does not exist.</returns>
    Task<ProjectDto?> GetBySlugAsync(string slug, string? language = null, CancellationToken ct = default);

    /// <param name="exceptId">Ignore this project (the one being updated).</param>
    Task<bool> IsSlugTakenAsync(string slug, int? exceptId = null, CancellationToken ct = default);

    Task<ProjectDto> CreateAsync(ProjectRequest request, CancellationToken ct = default);

    /// <returns>null if the project does not exist.</returns>
    Task<ProjectDto?> UpdateAsync(int id, ProjectRequest request, CancellationToken ct = default);

    /// <returns>false if the project does not exist.</returns>
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
