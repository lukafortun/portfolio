using Portfolio.Api.DTOs;
using Portfolio.Api.Entities;

namespace Portfolio.Api.Services;

/// <summary>Tags shared by projects and experience entries.</summary>
public interface ITagService
{
    /// <summary>Every tag in use, most used first.</summary>
    Task<IReadOnlyList<TagDto>> GetAllAsync(CancellationToken ct = default);

    /// <summary>
    /// Makes <paramref name="tags"/> (a project's or an entry's list) match <paramref name="requested"/>:
    /// existing tags are reused, missing ones created, the others unlinked. Saved with the owner's SaveChanges.
    /// </summary>
    Task SyncAsync(List<Tag> tags, IEnumerable<string>? requested, CancellationToken ct = default);

    /// <summary>Deletes the tags nothing uses anymore. Call it after saving a change that unlinked tags.</summary>
    Task RemoveUnusedAsync(CancellationToken ct = default);
}
