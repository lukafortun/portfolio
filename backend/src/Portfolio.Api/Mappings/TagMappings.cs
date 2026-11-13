using Portfolio.Api.Entities;

namespace Portfolio.Api.Mappings;

public static class TagMappings
{
    /// <summary>Tags as sent to the frontend: names only, alphabetically (a join table has no order).</summary>
    public static List<string> ToNames(this IEnumerable<Tag> tags) => tags.Select(t => t.Name).Order().ToList();
}
