using System.ComponentModel.DataAnnotations;
using Portfolio.Api.Common;

namespace Portfolio.Api.DTOs;

// Translated texts are sent with every language, { "fr": "…", "en": "…" }: the frontend picks the one it shows.

// ---------- Profile ----------

public record ProfileLinkDto(
    [Required, StringLength(50)] string Label,
    [Required, Url, StringLength(500)] string Url);

public record ProfileDto(
    string Name,
    IReadOnlyDictionary<string, string> Role,
    IReadOnlyDictionary<string, string> Bio,
    IReadOnlyDictionary<string, string> Location,
    string? Email,
    IReadOnlyList<ProfileLinkDto> Links);

public record ProfileRequest(
    [Required, StringLength(100)] string Name,
    [LocalizedText(100)] Dictionary<string, string?> Role,
    [LocalizedText(2000)] Dictionary<string, string?> Bio,
    [LocalizedText(100, Required = false)] Dictionary<string, string?>? Location,
    [Required, MaxLength(10)] List<ProfileLinkDto> Links,
    [EmailAddress, StringLength(200)] string? Email = null);

// ---------- Skills ----------

public record SkillGroupDto(
    int Id,
    string Name,
    int SortOrder,
    IReadOnlyDictionary<string, string> Description,
    IReadOnlyList<string> Skills);

public record SkillGroupRequest(
    [Required, StringLength(50)] string Name,
    [Range(0, 1000)] int SortOrder,
    [LocalizedText(300, Required = false)] Dictionary<string, string?>? Description,
    [Required, MaxLength(50)] List<string> Skills) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) =>
        Tags.Validate(Skills, nameof(Skills));
}

// ---------- Experience ----------

public record ExperienceDto(
    int Id,
    string Start,
    string? End,
    string Company,
    IReadOnlyDictionary<string, string> Role,
    IReadOnlyDictionary<string, string> Location,
    IReadOnlyDictionary<string, string> Description,
    IReadOnlyList<string> Tags);

public record ExperienceRequest(
    [Required, RegularExpression(ExperienceRequest.YearMonth, ErrorMessage = ExperienceRequest.YearMonthError)]
    string Start,
    [RegularExpression(ExperienceRequest.YearMonth, ErrorMessage = ExperienceRequest.YearMonthError)]
    string? End,
    [Required, StringLength(100)] string Company,
    [LocalizedText(100)] Dictionary<string, string?> Role,
    [LocalizedText(100, Required = false)] Dictionary<string, string?>? Location,
    [LocalizedText(2000, Required = false)] Dictionary<string, string?>? Description,
    [MaxLength(20)] List<string>? Tags) : IValidatableObject
{
    /// <summary>"2024-09"</summary>
    public const string YearMonth = @"^\d{4}-(0[1-9]|1[0-2])$";
    private const string YearMonthError = "Dates must be in the YYYY-MM format.";

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // "YYYY-MM" strings compare like dates.
        if (End is not null && string.CompareOrdinal(End, Start) < 0)
            yield return new ValidationResult("The end date must be after the start date.", [nameof(End)]);

        foreach (var error in DTOs.Tags.Validate(Tags, nameof(Tags)))
            yield return error;
    }
}

/// <summary>Shared rules for lists of short labels (skills, tags).</summary>
internal static class Tags
{
    public const int MaxLength = 50;

    public static IEnumerable<ValidationResult> Validate(IEnumerable<string>? items, string member)
    {
        if (items is null) yield break;
        if (items.Any(string.IsNullOrWhiteSpace))
            yield return new ValidationResult($"{member} cannot contain empty items.", [member]);
        if (items.Any(i => i?.Length > MaxLength))
            yield return new ValidationResult($"Each item of {member} must be at most {MaxLength} characters.", [member]);
    }
}
