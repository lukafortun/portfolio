using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.DTOs;
using Portfolio.Api.Services;

namespace Portfolio.Api.Controllers;

/// <summary>
/// Tags of the projects and experience entries. Read-only: tags are created and deleted
/// with the projects and entries that use them.
/// </summary>
[ApiController]
[Route("api/tags")]
public class TagsController(ITagService tagService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TagDto>>> GetAll(CancellationToken ct) =>
        Ok(await tagService.GetAllAsync(ct));
}
