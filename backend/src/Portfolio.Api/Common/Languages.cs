namespace Portfolio.Api.Common;

public static class Languages
{
    /// <summary>Every project must have this translation; used as the fallback.</summary>
    public const string Default = "fr";

    public static readonly string[] All = ["fr", "en"];

    public static bool IsSupported(string? language) => language is not null && All.Contains(language);

    /// <summary>"en-US" → "en"; null or unknown → <see cref="Default"/>.</summary>
    public static string Normalize(string? language)
    {
        var code = language?.Split('-')[0].ToLowerInvariant();
        return IsSupported(code) ? code! : Default;
    }
}
