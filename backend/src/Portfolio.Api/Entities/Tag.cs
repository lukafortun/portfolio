namespace Portfolio.Api.Entities;

/// <summary>
/// A project tag ("react", "docker"). A tag is shared by every project that uses it:
/// projects and tags are linked through the ProjectTags join table (ProjectId, TagId).
/// </summary>
public class Tag
{
    public int Id { get; set; }

    /// <summary>Lowercase, without "#", no spaces. Unique.</summary>
    public required string Name { get; set; }

    public List<Project> Projects { get; set; } = [];
}
