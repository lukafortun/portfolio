using Portfolio.Api.Entities;

namespace Portfolio.Api.Repositories;

public interface IProjectRepository
{
    Task<List<Project>> GetAllAsync(CancellationToken ct = default);
    Task<Project?> GetByIdAsync(int id, CancellationToken ct = default);
    Task AddAsync(Project project, CancellationToken ct = default);
    void Remove(Project project);
    Task SaveChangesAsync(CancellationToken ct = default);
}
