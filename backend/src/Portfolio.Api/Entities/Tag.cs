namespace Portfolio.Api.Entities;

/// <summary>
/// A tag ("react", "docker"), shared by projects and experience entries: one row per tag,
/// linked through the ProjectTags (ProjectId, TagId) and ExperienceTags (ExperienceId, TagId) join tables.
/// </summary>
public class Tag
{
    public int Id { get; set; }

    /// <summary>Lowercase, without "#", no spaces. Unique.</summary>
    public required string Name { get; set; }

    public List<Project> Projects { get; set; } = [];
    public List<Experience> Experiences { get; set; } = [];
}
