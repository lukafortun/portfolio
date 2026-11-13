namespace Portfolio.Api.Entities;

/// <summary>The site owner, shown on the home page. There is a single row.</summary>
public class Profile
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    // Translated texts: { "fr": "…", "en": "…" }, stored as JSON.
    public Dictionary<string, string> Role { get; set; } = [];
    public Dictionary<string, string> Bio { get; set; } = [];
    public Dictionary<string, string> Location { get; set; } = [];

    /// <summary>Contact address, shown as a mailto link. Optional.</summary>
    public string? Email { get; set; }

    /// <summary>Stored as JSON.</summary>
    public List<ProfileLink> Links { get; set; } = [];
}

public class ProfileLink
{
    public string Label { get; set; } = "";
    public string Url { get; set; } = "";
}
