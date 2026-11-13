using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.DTOs;
using Portfolio.Api.Extensions;
using Portfolio.Api.Services;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("api/skills")]
public class SkillsController(ISkillService skillService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SkillGroupDto>>> GetAll(CancellationToken ct) =>
        Ok(await skillService.GetAllAsync(ct));

    [HttpPost]
    [Authorize(Policy = AuthSetup.AdminPolicy)]
    public async Task<ActionResult<SkillGroupDto>> Create(SkillGroupRequest request, CancellationToken ct)
    {
        var group = await skillService.CreateAsync(request, ct);
        return Created($"/api/skills/{group.Id}", group);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = AuthSetup.AdminPolicy)]
    public async Task<ActionResult<SkillGroupDto>> Update(int id, SkillGroupRequest request, CancellationToken ct) =>
        await skillService.UpdateAsync(id, request, ct) is { } group ? Ok(group) : NotFound();

    [HttpDelete("{id:int}")]
    [Authorize(Policy = AuthSetup.AdminPolicy)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct) =>
        await skillService.DeleteAsync(id, ct) ? NoContent() : NotFound();
}
