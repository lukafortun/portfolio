namespace Portfolio.Api.DTOs;

/// <summary>A tag and where it is used (suggestions in the admin forms, links on the experience page).</summary>
public record TagDto(string Name, int ProjectCount, int ExperienceCount);
