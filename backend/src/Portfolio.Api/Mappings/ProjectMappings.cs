using Portfolio.Api.Common;
using Portfolio.Api.DTOs;
using Portfolio.Api.Entities;

namespace Portfolio.Api.Mappings;

public static class ProjectMappings
{
    /// <param name="language">Language to show; falls back to the default language.</param>
    public static ProjectListItemDto ToListItem(this Project p, string? language = null)
    {
        var t = p.Translate(language);
        return new ProjectListItemDto(
            p.Id, p.Slug, t.Language, t.Title, t.Summary, p.RepoUrl, p.LiveUrl, p.CoverImage, p.CreatedAt,
            p.OrderedTranslations().Select(x => x.Language).ToList(),
            p.TagNames());
    }

    /// <param name="language">Language to show; falls back to the default language.</param>
    public static ProjectDto ToDto(this Project p, string? language = null)
    {
        var t = p.Translate(language);
        return new ProjectDto(
            p.Id, p.Slug, t.Language, t.Title, t.Summary, t.Content, p.RepoUrl, p.LiveUrl, p.CoverImage, p.CreatedAt,
            p.OrderedTranslations()
                .Select(x => new ProjectTranslationDto(x.Language, x.Title, x.Summary, x.Content))
                .ToList(),
            p.TagNames());
    }

    /// <summary>A join table has no order: tags are returned alphabetically.</summary>
    private static List<string> TagNames(this Project p) => p.Tags.Select(t => t.Name).Order().ToList();

    /// <summary>
    /// Tags are used in URLs (?tag=react) and compared as-is, so they get one canonical form:
    /// "#React", " react " and "React" all become "react"; spaces become dashes ("Spring Boot" → "spring-boot").
    /// </summary>
    public static List<string> NormalizeTags(IEnumerable<string>? tags) =>
        (tags ?? [])
            .Select(t => string.Join('-', t.Trim().TrimStart('#').ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries)))
            .Where(t => t.Length > 0)
            .Distinct()
            .ToList();

    /// <summary>Requested language → default language → any translation.</summary>
    private static ProjectTranslation Translate(this Project p, string? language)
    {
        var code = Languages.Normalize(language);
        return p.Translations.FirstOrDefault(t => t.Language == code)
            ?? p.Translations.FirstOrDefault(t => t.Language == Languages.Default)
            ?? p.Translations.FirstOrDefault()
            ?? new ProjectTranslation { Language = Languages.Default, Title = "", Summary = "" };
    }

    private static IEnumerable<ProjectTranslation> OrderedTranslations(this Project p) =>
        p.Translations.OrderBy(t => Array.IndexOf(Languages.All, t.Language));

    public static Project ToEntity(this ProjectRequest r) => new()
    {
        Slug = r.Slug,
        RepoUrl = r.RepoUrl,
        LiveUrl = r.LiveUrl,
        CoverImage = r.CoverImage,
        // Tags are set by ProjectService: existing Tag rows must be loaded from the database first.
        Translations = r.Translations
            .Select(t => new ProjectTranslation
            {
                Language = t.Language,
                Title = t.Title,
                Summary = t.Summary,
                Content = t.Content ?? "",
            })
            .ToList(),
    };

    public static void ApplyTo(this ProjectRequest r, Project p)
    {
        p.Slug = r.Slug;
        p.RepoUrl = r.RepoUrl;
        p.LiveUrl = r.LiveUrl;
        p.CoverImage = r.CoverImage;

        // Update rows in place instead of clearing the list: deleting and re-adding the same
        // (ProjectId, Language) key in one SaveChanges makes EF Core throw.
        p.Translations.RemoveAll(existing => r.Translations.All(t => t.Language != existing.Language));

        foreach (var t in r.Translations)
        {
            var existing = p.Translations.Find(x => x.Language == t.Language);
            if (existing is null)
            {
                existing = new ProjectTranslation { Language = t.Language, Title = t.Title, Summary = t.Summary };
                p.Translations.Add(existing);
            }

            existing.Title = t.Title;
            existing.Summary = t.Summary;
            existing.Content = t.Content ?? "";
        }
    }
}
