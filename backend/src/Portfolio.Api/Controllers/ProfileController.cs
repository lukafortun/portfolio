using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.DTOs;
using Portfolio.Api.Extensions;
using Portfolio.Api.Services;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("api/profile")]
public class ProfileController(IProfileService profileService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ProfileDto>> Get(CancellationToken ct) =>
        Ok(await profileService.GetAsync(ct));

    [HttpPut]
    [Authorize(Policy = AuthSetup.AdminPolicy)]
    public async Task<ActionResult<ProfileDto>> Save(ProfileRequest request, CancellationToken ct) =>
        Ok(await profileService.SaveAsync(request, ct));
}
