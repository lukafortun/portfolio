using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Data;
using Portfolio.Api.DTOs;
using Portfolio.Api.Entities;

namespace Portfolio.Api.Repositories;

public class ProjectRepository(AppDbContext db) : IProjectRepository
{
    // Each project comes with its translations and its tags (Include → SQL JOINs).
    // AsSplitQuery: one SQL query per collection, instead of one JOIN that would repeat
    // every translation for every tag (translations × tags rows per project).
    private IQueryable<Project> WithDetails() =>
        db.Projects
            .Include(p => p.Translations)
            .Include(p => p.Tags)
            .AsSplitQuery();

    public Task<List<Project>> GetAllAsync(CancellationToken ct = default) =>
        WithDetails()
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(ct);

    public Task<Project?> GetByIdAsync(int id, CancellationToken ct = default) =>
        WithDetails().FirstOrDefaultAsync(p => p.Id == id, ct);

    public Task<Project?> GetBySlugAsync(string slug, CancellationToken ct = default) =>
        WithDetails()
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Slug == slug, ct);

    public Task<bool> SlugExistsAsync(string slug, int? exceptId = null, CancellationToken ct = default) =>
        db.Projects.AnyAsync(p => p.Slug == slug && p.Id != exceptId, ct);

    public async Task AddAsync(Project project, CancellationToken ct = default) =>
        await db.Projects.AddAsync(project, ct);

    public void Remove(Project project) => db.Projects.Remove(project);

    public Task SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);

    // WHERE "Name" IN (…)
    public Task<List<Tag>> GetTagsAsync(IReadOnlyCollection<string> names, CancellationToken ct = default) =>
        db.Tags.Where(t => names.Contains(t.Name)).ToListAsync(ct);

    // Projected straight to the DTO: SQL returns only the name and a COUNT, no entity is loaded.
    // Sorted before the Select: EF Core can't translate an OrderBy on the properties of a record built by Select.
    public Task<List<TagDto>> GetTagUsageAsync(CancellationToken ct = default) =>
        db.Tags
            .OrderByDescending(t => t.Projects.Count)
            .ThenBy(t => t.Name)
            .Select(t => new TagDto(t.Name, t.Projects.Count))
            .ToListAsync(ct);

    // A single DELETE … WHERE NOT EXISTS (… ProjectTags …), without loading the tags.
    public Task RemoveUnusedTagsAsync(CancellationToken ct = default) =>
        db.Tags.Where(t => t.Projects.Count == 0).ExecuteDeleteAsync(ct);
}
