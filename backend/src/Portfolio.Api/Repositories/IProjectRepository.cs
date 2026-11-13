using Portfolio.Api.DTOs;
using Portfolio.Api.Entities;

namespace Portfolio.Api.Repositories;

public interface IProjectRepository
{
    Task<List<Project>> GetAllAsync(CancellationToken ct = default);
    Task<Project?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Project?> GetBySlugAsync(string slug, CancellationToken ct = default);

    /// <param name="exceptId">Ignore this project (the one being updated).</param>
    Task<bool> SlugExistsAsync(string slug, int? exceptId = null, CancellationToken ct = default);

    Task AddAsync(Project project, CancellationToken ct = default);
    void Remove(Project project);
    Task SaveChangesAsync(CancellationToken ct = default);

    /// <summary>The tags among <paramref name="names"/> that already exist.</summary>
    Task<List<Tag>> GetTagsAsync(IReadOnlyCollection<string> names, CancellationToken ct = default);

    /// <summary>Every tag with its number of projects, most used first.</summary>
    Task<List<TagDto>> GetTagUsageAsync(CancellationToken ct = default);

    /// <summary>Deletes the tags no project uses anymore.</summary>
    Task RemoveUnusedTagsAsync(CancellationToken ct = default);
}
