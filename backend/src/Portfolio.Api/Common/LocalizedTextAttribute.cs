using System.ComponentModel.DataAnnotations;

namespace Portfolio.Api.Common;

/// <summary>
/// Validates a translated text, { "fr": "…", "en": "…" }: only supported languages, each at most
/// <paramref name="maxLength"/> characters, and (if <see cref="Required"/>) a non-empty default-language text.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.Field)]
public sealed class LocalizedTextAttribute(int maxLength) : ValidationAttribute
{
    /// <summary>Whether the default language ("fr") must be filled in. Other languages are always optional.</summary>
    public bool Required { get; set; } = true;

    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        var name = context.DisplayName;
        string[]? member = context.MemberName is { } m ? [m] : null;

        if (value is not IDictionary<string, string?> text)
        {
            return Required ? new ValidationResult($"The {name} field is required.", member) : ValidationResult.Success;
        }

        foreach (var (language, translation) in text)
        {
            if (!Languages.IsSupported(language))
                return new ValidationResult($"{name}: unsupported language '{language}'.", member);
            if (translation?.Length > maxLength)
                return new ValidationResult($"{name} ({language}) must be at most {maxLength} characters.", member);
        }

        if (Required && (!text.TryGetValue(Languages.Default, out var defaultText) || string.IsNullOrWhiteSpace(defaultText)))
            return new ValidationResult($"{name}: a '{Languages.Default}' text is required.", member);

        return ValidationResult.Success;
    }
}
