using System.ComponentModel.DataAnnotations;
using Portfolio.Api.Common;
using Portfolio.Api.Services;

namespace Portfolio.Api.DTOs;

public record ProjectTranslationDto(
    [Required, StringLength(5)] string Language,
    [Required, StringLength(200)] string Title,
    [Required, StringLength(500)] string Summary,
    [StringLength(100_000)] string? Content);

/// <summary>A project in the list (no Markdown content, to keep the response small).</summary>
/// <param name="Language">Language of Title and Summary: the requested one, or the default if it has no translation.</param>
/// <param name="Languages">Languages this project is translated into.</param>
public record ProjectListItemDto(
    int Id,
    string Slug,
    string Language,
    string Title,
    string Summary,
    string? RepoUrl,
    string? LiveUrl,
    string? CoverImage,
    DateTime CreatedAt,
    IReadOnlyList<string> Languages,
    IReadOnlyList<string> Tags);

/// <summary>A full project, for its page and for the admin form.</summary>
/// <param name="Language">Language of Title, Summary and Content (see <see cref="ProjectListItemDto"/>).</param>
/// <param name="Translations">Every translation, for the admin form.</param>
public record ProjectDto(
    int Id,
    string Slug,
    string Language,
    string Title,
    string Summary,
    string Content,
    string? RepoUrl,
    string? LiveUrl,
    string? CoverImage,
    DateTime CreatedAt,
    IReadOnlyList<ProjectTranslationDto> Translations,
    IReadOnlyList<string> Tags);

/// <summary>A tag and the number of projects using it (suggestions in the admin form).</summary>
public record TagDto(string Name, int ProjectCount);

public record ProjectRequest(
    [Required, StringLength(100), RegularExpression(ProjectRequest.SlugPattern, ErrorMessage = ProjectRequest.SlugError)]
    string Slug,
    [Required] IReadOnlyList<ProjectTranslationDto> Translations,
    [Url, StringLength(500)] string? RepoUrl,
    [Url, StringLength(500)] string? LiveUrl,
    [StringLength(500)] string? CoverImage = null,
    [MaxLength(15)] List<string>? Tags = null) : IValidatableObject
{
    /// <summary>Lowercase words separated by single dashes: "my-project-2".</summary>
    public const string SlugPattern = "^[a-z0-9]+(-[a-z0-9]+)*$";
    private const string SlugError = "The slug may only contain lowercase letters, digits and dashes (e.g. my-project).";

    // Rules spanning several translations, which attributes can't express. Errors become a 400.
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // An uploaded image (/api/uploads/…) or an absolute http(s) URL.
        if (CoverImage is not null && !IsUploadedImage(CoverImage) && !IsHttpUrl(CoverImage))
            yield return new ValidationResult("The cover image must be an uploaded image or an http(s) URL.", [nameof(CoverImage)]);

        foreach (var error in DTOs.Tags.Validate(Tags, nameof(Tags)))
            yield return error;

        if (Translations is null) yield break;
        var languages = Translations.Where(t => t is not null).Select(t => t.Language).ToList();
        string[] member = [nameof(Translations)];

        foreach (var language in languages.Where(l => !Languages.IsSupported(l)).Distinct())
            yield return new ValidationResult($"Unsupported language '{language}'.", member);

        if (languages.Count != languages.Distinct().Count())
            yield return new ValidationResult("Each language can only appear once.", member);

        if (!languages.Contains(Languages.Default))
            yield return new ValidationResult($"A '{Languages.Default}' translation is required.", member);
    }

    private static bool IsUploadedImage(string value) =>
        value.StartsWith($"{IImageStorage.PublicPath}/", StringComparison.Ordinal) && !value.Contains("..");

    private static bool IsHttpUrl(string value) =>
        Uri.TryCreate(value, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp);
}
