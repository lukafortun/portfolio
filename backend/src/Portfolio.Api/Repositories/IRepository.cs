namespace Portfolio.Api.Repositories;

/// <summary>
/// Basic data access for simple entities (no relations to load).
/// Projects keep their own repository: they need Include() for their translations and slug lookups.
/// </summary>
public interface IRepository<T> where T : class
{
    /// <summary>Read-only (not tracked): changes to these objects are not saved.</summary>
    Task<List<T>> GetAllAsync(CancellationToken ct = default);

    /// <summary>Tracked: change it, then call <see cref="SaveChangesAsync"/>.</summary>
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);

    void Add(T entity);
    void Remove(T entity);
    Task SaveChangesAsync(CancellationToken ct = default);
}
