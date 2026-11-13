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
            e.Property(p => p.Slug).HasMaxLength(100);
            e.HasIndex(p => p.Slug).IsUnique();
            e.Property(p => p.RepoUrl).HasMaxLength(500);
            e.Property(p => p.LiveUrl).HasMaxLength(500);
            e.Property(p => p.CoverImage).HasMaxLength(500);

            // One project → many translations; deleting a project deletes its translations.
            e.HasMany(p => p.Translations)
                .WithOne()
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ProjectTranslation>(e =>
        {
            e.ToTable("ProjectTranslations");
            e.HasKey(t => new { t.ProjectId, t.Language });
            e.Property(t => t.Language).HasMaxLength(5);
            e.Property(t => t.Title).HasMaxLength(200);
            e.Property(t => t.Summary).HasMaxLength(500);
            // Content has no max length: "text" column in PostgreSQL.
        });
    }
}
