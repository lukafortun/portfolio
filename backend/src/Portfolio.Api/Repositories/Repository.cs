using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Data;

namespace Portfolio.Api.Repositories;

/// <summary>
/// Generic implementation, registered once for every T:
/// services.AddScoped(typeof(IRepository&lt;&gt;), typeof(Repository&lt;&gt;)).
/// </summary>
public class Repository<T>(AppDbContext db) : IRepository<T> where T : class
{
    private DbSet<T> Set => db.Set<T>();

    public Task<List<T>> GetAllAsync(CancellationToken ct = default) => Set.AsNoTracking().ToListAsync(ct);

    public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default) => await Set.FindAsync([id], ct);

    public void Add(T entity) => Set.Add(entity);

    public void Remove(T entity) => Set.Remove(entity);

    public Task SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
}
