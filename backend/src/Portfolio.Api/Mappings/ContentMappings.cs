using Portfolio.Api.Common;
using Portfolio.Api.DTOs;
using Portfolio.Api.Entities;

namespace Portfolio.Api.Mappings;

public static class ContentMappings
{
    // ---------- Profile ----------

    public static ProfileDto ToDto(this Profile p) =>
        new(p.Name, p.Role, p.Bio, p.Location, p.Email, p.Links.Select(l => new ProfileLinkDto(l.Label, l.Url)).ToList());

    public static void ApplyTo(this ProfileRequest r, Profile p)
    {
        p.Name = r.Name.Trim();
        p.Role = Clean(r.Role);
        p.Bio = Clean(r.Bio);
        p.Location = Clean(r.Location);
        p.Email = string.IsNullOrWhiteSpace(r.Email) ? null : r.Email.Trim();
        p.Links = r.Links.Select(l => new ProfileLink { Label = l.Label.Trim(), Url = l.Url.Trim() }).ToList();
    }

    // ---------- Skills ----------

    public static SkillGroupDto ToDto(this SkillGroup g) => new(g.Id, g.Name, g.SortOrder, g.Description, g.Skills);

    public static void ApplyTo(this SkillGroupRequest r, SkillGroup g)
    {
        g.Name = r.Name.Trim();
        g.SortOrder = r.SortOrder;
        g.Description = Clean(r.Description);
        g.Skills = CleanList(r.Skills);
    }

    // ---------- Experience ----------

    public static ExperienceDto ToDto(this Experience x) =>
        new(x.Id, x.Start, x.End, x.Company, x.Role, x.Location, x.Description, x.Tags.ToNames());

    public static void ApplyTo(this ExperienceRequest r, Experience x)
    {
        x.Start = r.Start;
        x.End = r.End;
        x.Company = r.Company.Trim();
        x.Role = Clean(r.Role);
        x.Location = Clean(r.Location);
        x.Description = Clean(r.Description);
        // Tags are set by ExperienceService: existing Tag rows must be loaded from the database first.
    }

    // ---------- Helpers ----------

    /// <summary>Keeps supported languages with a non-empty text, trimmed. (Validation already rejected the rest.)</summary>
    private static Dictionary<string, string> Clean(Dictionary<string, string?>? text) =>
        (text ?? [])
            .Where(kv => Languages.IsSupported(kv.Key) && !string.IsNullOrWhiteSpace(kv.Value))
            .ToDictionary(kv => kv.Key, kv => kv.Value!.Trim());

    /// <summary>Trimmed, without duplicates, in the given order.</summary>
    private static List<string> CleanList(IEnumerable<string>? items) =>
        (items ?? []).Select(i => i.Trim()).Where(i => i.Length > 0).Distinct().ToList();
}
