using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Data;
using Portfolio.Api.DTOs;
using Portfolio.Api.Entities;

namespace Portfolio.Api.Repositories;

public class TagRepository(AppDbContext db) : ITagRepository
{
    // WHERE "Name" IN (…)
    public Task<List<Tag>> GetByNamesAsync(IReadOnlyCollection<string> names, CancellationToken ct = default) =>
        db.Tags.Where(t => names.Contains(t.Name)).ToListAsync(ct);

    // Projected straight to the DTO: SQL returns only the name and two COUNTs, no entity is loaded.
    // Sorted before the Select: EF Core can't translate an OrderBy on the properties of a record built by Select.
    public Task<List<TagDto>> GetUsageAsync(CancellationToken ct = default) =>
        db.Tags
            .OrderByDescending(t => t.Projects.Count + t.Experiences.Count)
            .ThenBy(t => t.Name)
            .Select(t => new TagDto(t.Name, t.Projects.Count, t.Experiences.Count))
            .ToListAsync(ct);

    // A single DELETE … WHERE NOT EXISTS (…), without loading the tags.
    public Task RemoveUnusedAsync(CancellationToken ct = default) =>
        db.Tags.Where(t => t.Projects.Count == 0 && t.Experiences.Count == 0).ExecuteDeleteAsync(ct);
}
