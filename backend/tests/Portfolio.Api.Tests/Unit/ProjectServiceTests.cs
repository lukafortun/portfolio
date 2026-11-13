using NSubstitute;
using Portfolio.Api.DTOs;
using Portfolio.Api.Entities;
using Portfolio.Api.Repositories;
using Portfolio.Api.Services;

namespace Portfolio.Api.Tests.Unit;

public class ProjectServiceTests
{
    private readonly IProjectRepository _repository = Substitute.For<IProjectRepository>();
    private readonly ProjectService _service;

    private static readonly ProjectRequest Request = new(
        "mon-projet",
        [new("fr", "Titre", "Résumé", "# Contenu"), new("en", "Title", "Summary", "# Content")],
        "https://github.com/x/y",
        null);

    public ProjectServiceTests() => _service = new ProjectService(_repository);

    private static Project ExistingProject() => new()
    {
        Id = 1,
        Slug = "ancien",
        Translations =
        [
            new() { ProjectId = 1, Language = "fr", Title = "Ancien", Summary = "Ancien résumé", Content = "Ancien contenu" },
            new() { ProjectId = 1, Language = "en", Title = "Old", Summary = "Old summary", Content = "Old content" },
        ],
    };

    [Fact]
    public async Task GetByIdAsync_returns_null_when_project_does_not_exist()
    {
        _repository.GetByIdAsync(42, Arg.Any<CancellationToken>()).Returns((Project?)null);

        var result = await _service.GetByIdAsync(42);

        Assert.Null(result);
    }

    [Theory]
    [InlineData("en", "en", "Old")]
    [InlineData("fr", "fr", "Ancien")]
    [InlineData("en-US", "en", "Old")]
    [InlineData(null, "fr", "Ancien")]
    [InlineData("de", "fr", "Ancien")]
    public async Task GetByIdAsync_returns_requested_language_or_default(string? requested, string expectedLanguage, string expectedTitle)
    {
        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(ExistingProject());

        var result = await _service.GetByIdAsync(1, requested);

        Assert.Equal(expectedLanguage, result!.Language);
        Assert.Equal(expectedTitle, result.Title);
    }

    [Fact]
    public async Task GetBySlugAsync_falls_back_to_default_language_when_translation_is_missing()
    {
        var project = ExistingProject();
        project.Translations.RemoveAll(t => t.Language == "en");
        _repository.GetBySlugAsync("ancien", Arg.Any<CancellationToken>()).Returns(project);

        var result = await _service.GetBySlugAsync("ancien", "en");

        Assert.Equal("fr", result!.Language);
        Assert.Equal("Ancien contenu", result.Content);
    }

    [Fact]
    public async Task GetAllAsync_returns_list_items_with_available_languages()
    {
        _repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns([ExistingProject()]);

        var item = Assert.Single(await _service.GetAllAsync("en"));

        Assert.Equal("Old summary", item.Summary);
        Assert.Equal(["fr", "en"], item.Languages);
    }

    [Fact]
    public async Task CreateAsync_adds_and_saves_then_returns_dto()
    {
        var result = await _service.CreateAsync(Request);

        await _repository.Received(1).AddAsync(
            Arg.Is<Project>(p => p.Slug == "mon-projet" && p.Translations.Count == 2),
            Arg.Any<CancellationToken>());
        await _repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        Assert.Equal("Titre", result.Title);
        Assert.Equal("# Contenu", result.Content);
        Assert.Equal(["fr", "en"], result.Translations.Select(t => t.Language));
    }

    [Fact]
    public async Task UpdateAsync_updates_slug_and_translations_in_place()
    {
        var existing = ExistingProject();
        var frRow = existing.Translations[0];
        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(existing);

        var result = await _service.UpdateAsync(1, Request);

        Assert.NotNull(result);
        Assert.Equal("mon-projet", existing.Slug);
        Assert.Same(frRow, existing.Translations.Single(t => t.Language == "fr"));
        Assert.Equal("# Contenu", frRow.Content);
        await _repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_reuses_existing_tags_and_creates_missing_ones()
    {
        var react = new Tag { Id = 7, Name = "react" };
        _repository.GetTagsAsync(Arg.Any<IReadOnlyCollection<string>>(), Arg.Any<CancellationToken>()).Returns([react]);

        await _service.CreateAsync(Request with { Tags = ["#React", "docker"] });

        await _repository.Received(1).AddAsync(
            Arg.Is<Project>(p => p.Tags.Count == 2
                && p.Tags.Contains(react)                                  // same row, not a second "react"
                && p.Tags.Any(t => t.Name == "docker" && t.Id == 0)),     // new row, inserted on save
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAsync_unlinks_removed_tags_then_deletes_unused_ones()
    {
        var existing = ExistingProject();
        existing.Tags = [new() { Id = 1, Name = "react" }, new() { Id = 2, Name = "vue" }];
        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(existing);

        await _service.UpdateAsync(1, Request with { Tags = ["react"] });

        Assert.Equal(["react"], existing.Tags.Select(t => t.Name));
        // Nothing to look up: "react" is already linked.
        await _repository.DidNotReceive().GetTagsAsync(Arg.Any<IReadOnlyCollection<string>>(), Arg.Any<CancellationToken>());
        Received.InOrder(() =>
        {
            _repository.SaveChangesAsync(Arg.Any<CancellationToken>());
            _repository.RemoveUnusedTagsAsync(Arg.Any<CancellationToken>());
        });
    }

    [Fact]
    public async Task UpdateAsync_removes_translations_missing_from_the_request()
    {
        var existing = ExistingProject();
        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(existing);

        await _service.UpdateAsync(1, Request with { Translations = [new("fr", "Titre", "Résumé", null)] });

        var remaining = Assert.Single(existing.Translations);
        Assert.Equal("fr", remaining.Language);
        Assert.Equal("", remaining.Content);
    }

    [Fact]
    public async Task UpdateAsync_returns_null_and_saves_nothing_when_missing()
    {
        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns((Project?)null);

        var result = await _service.UpdateAsync(1, Request);

        Assert.Null(result);
        await _repository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_returns_false_when_missing()
    {
        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns((Project?)null);

        Assert.False(await _service.DeleteAsync(1));
        _repository.DidNotReceive().Remove(Arg.Any<Project>());
    }

    [Fact]
    public async Task DeleteAsync_removes_existing_project()
    {
        var existing = ExistingProject();
        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(existing);

        Assert.True(await _service.DeleteAsync(1));
        _repository.Received(1).Remove(existing);
        await _repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
