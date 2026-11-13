using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Entities;

namespace Portfolio.Api.Data;

/// <summary>
/// Fills an empty database with example content, so a fresh install doesn't show a blank site.
/// Runs once: only while there is no profile yet (deleting skills or experiences won't bring them back).
/// Everything here is meant to be edited from the admin.
/// </summary>
public static class ContentSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken ct = default)
    {
        if (await db.Profiles.AnyAsync(ct)) return;

        db.Profiles.Add(new Profile
        {
            Id = 1,
            Name = "Prénom Nom",
            Role = new() { ["fr"] = "Développeur full-stack", ["en"] = "Full-stack developer" },
            Bio = new()
            {
                ["fr"] = "Je construis des applications web de bout en bout, de l’interface React à l’API ASP.NET Core et sa base PostgreSQL. J’aime le code propre, les outils bien réglés et comprendre comment les choses fonctionnent sous le capot.",
                ["en"] = "I build web applications end to end, from the React interface to the ASP.NET Core API and its PostgreSQL database. I like clean code, well-tuned tools and understanding how things work under the hood.",
            },
            Location = new() { ["fr"] = "France", ["en"] = "France" },
            Links =
            [
                new() { Label = "github", Url = "https://github.com/" },
                new() { Label = "linkedin", Url = "https://www.linkedin.com/" },
            ],
        });

        db.SkillGroups.AddRange(
            new SkillGroup
            {
                Name = "frontend",
                SortOrder = 10,
                Description = new() { ["fr"] = "Interfaces web rapides, typées et accessibles.", ["en"] = "Fast, typed and accessible web interfaces." },
                Skills = ["React", "TypeScript", "Vite", "React Router", "CSS Modules", "i18next"],
            },
            new SkillGroup
            {
                Name = "backend",
                SortOrder = 20,
                Description = new() { ["fr"] = "APIs REST structurées en couches, testées.", ["en"] = "Layered, tested REST APIs." },
                Skills = ["C#", "ASP.NET Core", "Entity Framework Core", "xUnit", "OpenID Connect"],
            },
            new SkillGroup
            {
                Name = "data",
                SortOrder = 30,
                Description = new() { ["fr"] = "Modélisation et migrations de bases relationnelles.", ["en"] = "Relational database modelling and migrations." },
                Skills = ["PostgreSQL", "SQL", "EF Core migrations"],
            },
            new SkillGroup
            {
                Name = "devops",
                SortOrder = 40,
                Description = new() { ["fr"] = "Conteneurs, déploiement et outillage.", ["en"] = "Containers, deployment and tooling." },
                Skills = ["Docker", "Docker Compose", "nginx", "Git", "Linux"],
            });

        // One Tag object per name, shared by every entry using it: two "new Tag { Name = "docker" }"
        // would insert two rows and break the unique index on Name.
        var tags = new Dictionary<string, Tag>();
        List<Tag> Tags(params string[] names) =>
            names.Select(name => tags.TryGetValue(name, out var tag) ? tag : tags[name] = new Tag { Name = name }).ToList();

        db.Experiences.AddRange(
            new Experience
            {
                Start = "2025-09",
                Company = "Entreprise",
                Role = new() { ["fr"] = "Développeur full-stack", ["en"] = "Full-stack developer" },
                Location = new() { ["fr"] = "Paris", ["en"] = "Paris" },
                Description = new()
                {
                    ["fr"] = "Développement d’applications web avec React et ASP.NET Core : conception d’API, base de données, tests et déploiement.",
                    ["en"] = "Building web applications with React and ASP.NET Core: API design, database, tests and deployment.",
                },
                Tags = Tags("react", "csharp", "dotnet", "postgresql"),
            },
            new Experience
            {
                Start = "2024-03",
                End = "2025-08",
                Company = "Autre entreprise",
                Role = new() { ["fr"] = "Stage développeur", ["en"] = "Developer intern" },
                Description = new()
                {
                    ["fr"] = "Participation au développement d’un outil interne, de la maquette à la mise en production.",
                    ["en"] = "Took part in building an internal tool, from mock-up to production.",
                },
                Tags = Tags("typescript", "docker"),
            },
            new Experience
            {
                Start = "2021-09",
                End = "2024-06",
                Company = "École",
                Role = new() { ["fr"] = "Formation en informatique", ["en"] = "Computer science studies" },
                Description = new()
                {
                    ["fr"] = "Algorithmique, programmation orientée objet, bases de données et réseaux.",
                    ["en"] = "Algorithms, object-oriented programming, databases and networking.",
                },
            });

        await db.SaveChangesAsync(ct);
    }
}
