using NSubstitute;
using Portfolio.Api.Entities;
using Portfolio.Api.Repositories;
using Portfolio.Api.Services;

namespace Portfolio.Api.Tests.Unit;

public class TagServiceTests
{
    private readonly ITagRepository _repository = Substitute.For<ITagRepository>();
    private readonly TagService _service;

    public TagServiceTests() => _service = new TagService(_repository);

    [Theory]
    [InlineData("React", "react")]
    [InlineData("#react", "react")]
    [InlineData("  ##React ", "react")]
    [InlineData("Spring  Boot", "spring-boot")]
    [InlineData("C#", "c#")]
    public void Normalize_gives_one_canonical_form(string input, string expected) =>
        Assert.Equal([expected], TagService.Normalize([input]));

    [Fact]
    public void Normalize_drops_empty_tags_and_duplicates() =>
        Assert.Equal(["react", "docker"], TagService.Normalize(["React", "#", "react", " ", "docker"]));

    [Fact]
    public async Task SyncAsync_reuses_existing_tags_and_creates_missing_ones()
    {
        var react = new Tag { Id = 7, Name = "react" };
        _repository.GetByNamesAsync(Arg.Any<IReadOnlyCollection<string>>(), Arg.Any<CancellationToken>()).Returns([react]);
        var tags = new List<Tag>();

        await _service.SyncAsync(tags, ["#React", "docker"]);

        Assert.Equal(2, tags.Count);
        Assert.Contains(react, tags);                                  // the same row, not a second "react"
        Assert.Contains(tags, t => t.Name == "docker" && t.Id == 0);   // a new row, inserted on save
    }

    [Fact]
    public async Task SyncAsync_unlinks_tags_missing_from_the_request()
    {
        var tags = new List<Tag> { new() { Id = 1, Name = "react" }, new() { Id = 2, Name = "vue" } };

        await _service.SyncAsync(tags, ["react"]);

        Assert.Equal(["react"], tags.Select(t => t.Name));
        // Nothing to look up: "react" is already linked.
        await _repository.DidNotReceive().GetByNamesAsync(Arg.Any<IReadOnlyCollection<string>>(), Arg.Any<CancellationToken>());
    }
}
