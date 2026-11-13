using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Data;

namespace Portfolio.Api.Repositories;

/// <summary>
/// Generic implementation, registered once for every T:
/// services.AddScoped(typeof(IRepository&lt;&gt;), typeof(Repository&lt;&gt;)).
/// An entity with relations to load derives from it and overrides the reads (see ExperienceRepository).
/// </summary>
public class Repository<T>(AppDbContext db) : IRepository<T> where T : class
{
    protected AppDbContext Db { get; } = db;
    protected DbSet<T> Set => Db.Set<T>();

    public virtual Task<List<T>> GetAllAsync(CancellationToken ct = default) => Set.AsNoTracking().ToListAsync(ct);

    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken ct = default) => await Set.FindAsync([id], ct);

    public void Add(T entity) => Set.Add(entity);

    public void Remove(T entity) => Set.Remove(entity);

    public Task SaveChangesAsync(CancellationToken ct = default) => Db.SaveChangesAsync(ct);
}
