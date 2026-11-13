using Portfolio.Api.DTOs;
using Portfolio.Api.Entities;
using Portfolio.Api.Repositories;

namespace Portfolio.Api.Services;

public class TagService(ITagRepository repository) : ITagService
{
    public async Task<IReadOnlyList<TagDto>> GetAllAsync(CancellationToken ct = default) =>
        await repository.GetUsageAsync(ct);

    public async Task SyncAsync(List<Tag> tags, IEnumerable<string>? requested, CancellationToken ct = default)
    {
        var names = Normalize(requested);

        tags.RemoveAll(tag => !names.Contains(tag.Name));

        var missing = names.Except(tags.Select(tag => tag.Name)).ToList();
        if (missing.Count == 0) return;

        // One "react" row shared by everything tagged react: reuse it if it exists.
        var existing = await repository.GetByNamesAsync(missing, ct);
        foreach (var name in missing)
            tags.Add(existing.Find(tag => tag.Name == name) ?? new Tag { Name = name });
    }

    public Task RemoveUnusedAsync(CancellationToken ct = default) => repository.RemoveUnusedAsync(ct);

    /// <summary>
    /// Tags are compared and used in URLs (?tag=react) as-is, so they get one canonical form:
    /// "#React", " react " and "React" all become "react"; spaces become dashes ("Spring Boot" → "spring-boot").
    /// Same rule as normalizeTag() in the frontend.
    /// </summary>
    public static List<string> Normalize(IEnumerable<string>? tags) =>
        (tags ?? [])
            .Select(t => string.Join('-', t.Trim().TrimStart('#').ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries)))
            .Where(t => t.Length > 0)
            .Distinct()
            .ToList();
}
