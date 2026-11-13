namespace Portfolio.Api.Entities;

public class Project
{
    public int Id { get; set; }

    /// <summary>URL identifier, e.g. "my-project" → /projects/my-project. Unique.</summary>
    public required string Slug { get; set; }

    public string? RepoUrl { get; set; }
    public string? LiveUrl { get; set; }

    /// <summary>Image shown on the card and at the top of the page: an uploaded image or an external URL.</summary>
    public string? CoverImage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Title, summary and content, one row per language (table ProjectTranslations).</summary>
    public List<ProjectTranslation> Translations { get; set; } = [];
}
