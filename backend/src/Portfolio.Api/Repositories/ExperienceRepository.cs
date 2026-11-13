using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Data;
using Portfolio.Api.Entities;

namespace Portfolio.Api.Repositories;

/// <summary>The generic repository, plus the tags of each entry (Include → SQL JOIN on ExperienceTags).</summary>
public class ExperienceRepository(AppDbContext db) : Repository<Experience>(db)
{
    public override Task<List<Experience>> GetAllAsync(CancellationToken ct = default) =>
        Set.AsNoTracking().Include(x => x.Tags).ToListAsync(ct);

    public override Task<Experience?> GetByIdAsync(int id, CancellationToken ct = default) =>
        Set.Include(x => x.Tags).FirstOrDefaultAsync(x => x.Id == id, ct);
}
