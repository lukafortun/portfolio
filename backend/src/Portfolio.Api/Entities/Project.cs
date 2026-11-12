namespace Portfolio.Api.Entities;

public class Project
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public string? RepoUrl { get; set; }
    public string? LiveUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
