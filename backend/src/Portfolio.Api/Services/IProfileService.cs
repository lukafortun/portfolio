using Portfolio.Api.DTOs;

namespace Portfolio.Api.Services;

public interface IProfileService
{
    /// <returns>The profile, or an empty one if it was never saved.</returns>
    Task<ProfileDto> GetAsync(CancellationToken ct = default);

    /// <summary>Creates the profile the first time, updates it afterwards.</summary>
    Task<ProfileDto> SaveAsync(ProfileRequest request, CancellationToken ct = default);
}
