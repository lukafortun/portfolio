using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Data;
using Portfolio.Api.Entities;

namespace Portfolio.Api.Repositories;

public class ProjectRepository(AppDbContext db) : IProjectRepository
{
    public Task<List<Project>> GetAllAsync(CancellationToken ct = default) =>
        db.Projects.AsNoTracking().OrderByDescending(p => p.CreatedAt).ToListAsync(ct);

    public Task<Project?> GetByIdAsync(int id, CancellationToken ct = default) =>
        db.Projects.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task AddAsync(Project project, CancellationToken ct = default) =>
        await db.Projects.AddAsync(project, ct);

    public void Remove(Project project) => db.Projects.Remove(project);

    public Task SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
}
