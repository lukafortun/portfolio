using Portfolio.Api.DTOs;
using Portfolio.Api.Entities;

namespace Portfolio.Api.Repositories;

public interface ITagRepository
{
    /// <summary>The tags among <paramref name="names"/> that already exist.</summary>
    Task<List<Tag>> GetByNamesAsync(IReadOnlyCollection<string> names, CancellationToken ct = default);

    /// <summary>Every tag with its number of projects and experience entries, most used first.</summary>
    Task<List<TagDto>> GetUsageAsync(CancellationToken ct = default);

    /// <summary>Deletes the tags no project and no experience entry uses anymore.</summary>
    Task RemoveUnusedAsync(CancellationToken ct = default);
}
