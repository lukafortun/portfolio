using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.DTOs;
using Portfolio.Api.Extensions;
using Portfolio.Api.Services;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsController(IProjectService projectService) : ControllerBase
{
    /// <param name="lang">Language of the texts, e.g. "en" (default: "fr").</param>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProjectListItemDto>>> GetAll([FromQuery] string? lang, CancellationToken ct) =>
        Ok(await projectService.GetAllAsync(lang, ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjectDto>> GetById(int id, [FromQuery] string? lang, CancellationToken ct) =>
        await projectService.GetByIdAsync(id, lang, ct) is { } project ? Ok(project) : NotFound();

    [HttpGet("by-slug/{slug}")]
    public async Task<ActionResult<ProjectDto>> GetBySlug(string slug, [FromQuery] string? lang, CancellationToken ct) =>
        await projectService.GetBySlugAsync(slug, lang, ct) is { } project ? Ok(project) : NotFound();

    [HttpPost]
    [Authorize(Policy = AuthSetup.AdminPolicy)]
    public async Task<ActionResult<ProjectDto>> Create(ProjectRequest request, CancellationToken ct)
    {
        if (await projectService.IsSlugTakenAsync(request.Slug, ct: ct)) return SlugConflict(request.Slug);

        var project = await projectService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = AuthSetup.AdminPolicy)]
    public async Task<ActionResult<ProjectDto>> Update(int id, ProjectRequest request, CancellationToken ct)
    {
        if (await projectService.IsSlugTakenAsync(request.Slug, exceptId: id, ct)) return SlugConflict(request.Slug);

        return await projectService.UpdateAsync(id, request, ct) is { } project ? Ok(project) : NotFound();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = AuthSetup.AdminPolicy)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct) =>
        await projectService.DeleteAsync(id, ct) ? NoContent() : NotFound();

    // 409 Conflict, as ProblemDetails (the frontend shows "detail").
    private ObjectResult SlugConflict(string slug) =>
        Problem(
            statusCode: StatusCodes.Status409Conflict,
            title: "Slug already used",
            detail: $"Another project already uses the slug '{slug}'.");
}
