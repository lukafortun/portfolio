using Microsoft.EntityFrameworkCore;

namespace Portfolio.Api.Data;

/// <summary>
/// Runs on startup: applies pending migrations when Database:MigrateOnStartup is true,
/// then fills an empty database with example content (see <see cref="ContentSeeder"/>).
/// </summary>
public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        if (db.Database.IsNpgsql())
        {
            if (!config.GetValue<bool>("Database:MigrateOnStartup")) return;
            await db.Database.MigrateAsync();
        }
        else
        {
            // Other providers (SQLite in tests): migrations are Npgsql-specific, so build the schema from the model.
            await db.Database.EnsureCreatedAsync();
        }

        await ContentSeeder.SeedAsync(db);
    }
}
