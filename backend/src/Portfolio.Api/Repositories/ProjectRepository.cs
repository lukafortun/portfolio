using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Data;
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
}
