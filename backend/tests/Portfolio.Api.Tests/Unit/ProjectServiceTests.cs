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

    private static readonly ProjectRequest Request = new("Titre", "Description", "https://github.com/x/y", null);

    public ProjectServiceTests() => _service = new ProjectService(_repository);

    [Fact]
    public async Task GetByIdAsync_returns_null_when_project_does_not_exist()
    {
        _repository.GetByIdAsync(42, Arg.Any<CancellationToken>()).Returns((Project?)null);

        var result = await _service.GetByIdAsync(42);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_adds_and_saves_then_returns_dto()
    {
        var result = await _service.CreateAsync(Request);

        await _repository.Received(1).AddAsync(Arg.Is<Project>(p => p.Title == "Titre"), Arg.Any<CancellationToken>());
        await _repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        Assert.Equal("Titre", result.Title);
        Assert.Equal("https://github.com/x/y", result.RepoUrl);
    }

    [Fact]
    public async Task UpdateAsync_modifies_existing_project()
    {
        var existing = new Project { Id = 1, Title = "Ancien", Description = "Ancienne" };
        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(existing);

        var result = await _service.UpdateAsync(1, Request);

        Assert.NotNull(result);
        Assert.Equal("Titre", existing.Title);
        await _repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
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
        var existing = new Project { Id = 1, Title = "T", Description = "D" };
        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(existing);

        Assert.True(await _service.DeleteAsync(1));
        _repository.Received(1).Remove(existing);
        await _repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
