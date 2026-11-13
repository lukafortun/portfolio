using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.DTOs;
using Portfolio.Api.Services;

namespace Portfolio.Api.Controllers;

/// <summary>Project tags. Read-only: tags are created and deleted with the projects that use them.</summary>
[ApiController]
[Route("api/tags")]
public class TagsController(IProjectService projectService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TagDto>>> GetAll(CancellationToken ct) =>
        Ok(await projectService.GetTagsAsync(ct));
}
