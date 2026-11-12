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
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProjectDto>>> GetAll(CancellationToken ct) =>
        Ok(await projectService.GetAllAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjectDto>> GetById(int id, CancellationToken ct) =>
        await projectService.GetByIdAsync(id, ct) is { } project ? Ok(project) : NotFound();

    [HttpPost]
    [Authorize(Policy = AuthSetup.AdminPolicy)]
    public async Task<ActionResult<ProjectDto>> Create(ProjectRequest request, CancellationToken ct)
    {
        var project = await projectService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = AuthSetup.AdminPolicy)]
    public async Task<ActionResult<ProjectDto>> Update(int id, ProjectRequest request, CancellationToken ct) =>
        await projectService.UpdateAsync(id, request, ct) is { } project ? Ok(project) : NotFound();

    [HttpDelete("{id:int}")]
    [Authorize(Policy = AuthSetup.AdminPolicy)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct) =>
        await projectService.DeleteAsync(id, ct) ? NoContent() : NotFound();
}
