using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Entities;

namespace Portfolio.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Project> Projects => Set<Project>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>(e =>
        {
            e.Property(p => p.Title).HasMaxLength(200);
            e.Property(p => p.Description).HasMaxLength(4000);
            e.Property(p => p.RepoUrl).HasMaxLength(500);
            e.Property(p => p.LiveUrl).HasMaxLength(500);
        });

    }
}
