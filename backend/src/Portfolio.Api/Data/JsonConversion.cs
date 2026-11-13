using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Portfolio.Api.Data;

public static class JsonConversion
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    /// <summary>
    /// Stores the property (dictionary, list…) as a JSON text column. Works the same in PostgreSQL and SQLite.
    /// </summary>
    public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> property) where T : class, new()
    {
        // EF Core compares values to detect changes: for a list or dictionary, compare the JSON,
        // otherwise adding an item to the same List instance would go unnoticed.
        var comparer = new ValueComparer<T>(
            (a, b) => Serialize(a) == Serialize(b),
            v => Serialize(v).GetHashCode(),
            v => Deserialize<T>(Serialize(v)));

        property.HasConversion(v => Serialize(v), json => Deserialize<T>(json), comparer);
        return property;
    }

    private static string Serialize<T>(T? value) => JsonSerializer.Serialize(value, Options);

    private static T Deserialize<T>(string json) where T : new() =>
        JsonSerializer.Deserialize<T>(json, Options) ?? new T();
}
