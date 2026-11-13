using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.DTOs;
using Portfolio.Api.Extensions;
using Portfolio.Api.Services;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("api/experiences")]
public class ExperiencesController(IExperienceService experienceService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ExperienceDto>>> GetAll(CancellationToken ct) =>
        Ok(await experienceService.GetAllAsync(ct));

    [HttpPost]
    [Authorize(Policy = AuthSetup.AdminPolicy)]
    public async Task<ActionResult<ExperienceDto>> Create(ExperienceRequest request, CancellationToken ct)
    {
        var experience = await experienceService.CreateAsync(request, ct);
        return Created($"/api/experiences/{experience.Id}", experience);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = AuthSetup.AdminPolicy)]
    public async Task<ActionResult<ExperienceDto>> Update(int id, ExperienceRequest request, CancellationToken ct) =>
        await experienceService.UpdateAsync(id, request, ct) is { } experience ? Ok(experience) : NotFound();

    [HttpDelete("{id:int}")]
    [Authorize(Policy = AuthSetup.AdminPolicy)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct) =>
        await experienceService.DeleteAsync(id, ct) ? NoContent() : NotFound();
}
