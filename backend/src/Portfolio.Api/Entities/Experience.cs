namespace Portfolio.Api.Entities;

/// <summary>A job, internship or education entry.</summary>
public class Experience
{
    public int Id { get; set; }

    /// <summary>Year and month, "2024-09". Sorts correctly as text.</summary>
    public string Start { get; set; } = "";

    /// <summary>"2025-06", or null if ongoing.</summary>
    public string? End { get; set; }

    public string Company { get; set; } = "";

    // Translated texts: { "fr": "…", "en": "…" }, stored as JSON.
    public Dictionary<string, string> Role { get; set; } = [];
    public Dictionary<string, string> Location { get; set; } = [];
    public Dictionary<string, string> Description { get; set; } = [];

    /// <summary>Stored as JSON.</summary>
    public List<string> Tags { get; set; } = [];
}
