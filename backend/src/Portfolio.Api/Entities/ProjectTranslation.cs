namespace Portfolio.Api.Entities;

/// <summary>The translatable text of a project. Primary key: (ProjectId, Language).</summary>
public class ProjectTranslation
{
    public int ProjectId { get; set; }
    public required string Language { get; set; }
    public required string Title { get; set; }

    /// <summary>Short plain text, shown on the project cards.</summary>
    public required string Summary { get; set; }

    /// <summary>Full page, in Markdown. May be empty.</summary>
    public string Content { get; set; } = "";
}
