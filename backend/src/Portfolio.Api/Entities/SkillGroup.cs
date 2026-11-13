namespace Portfolio.Api.Entities;

/// <summary>A group of skills, e.g. "frontend": React, TypeScript…</summary>
public class SkillGroup
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    /// <summary>Display order, ascending.</summary>
    public int SortOrder { get; set; }

    /// <summary>Translated: { "fr": "…", "en": "…" }, stored as JSON.</summary>
    public Dictionary<string, string> Description { get; set; } = [];

    /// <summary>Stored as JSON.</summary>
    public List<string> Skills { get; set; } = [];
}
