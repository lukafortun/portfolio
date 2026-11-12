using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Services;

namespace Portfolio.Api.Data;

/// <summary>Runs on startup: applies pending migrations when Database:MigrateOnStartup is true.</summary>
public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(DatabaseInitializer));

        if (db.Database.IsNpgsql())
        {
            if (config.GetValue<bool>("Database:MigrateOnStartup"))
                await db.Database.MigrateAsync();
        }
        else
        {
            // Other providers (SQLite in tests): migrations are Npgsql-specific, so build the schema from the model.
            await db.Database.EnsureCreatedAsync();
        }

    }
}
