using Portfolio.Api.DTOs;
using Portfolio.Api.Entities;
using Portfolio.Api.Mappings;
using Portfolio.Api.Repositories;

namespace Portfolio.Api.Services;

public class ProfileService(IRepository<Profile> repository) : IProfileService
{
    // There is a single profile: always row 1.
    private const int ProfileId = 1;

    public async Task<ProfileDto> GetAsync(CancellationToken ct = default) =>
        (await repository.GetByIdAsync(ProfileId, ct) ?? new Profile()).ToDto();

    public async Task<ProfileDto> SaveAsync(ProfileRequest request, CancellationToken ct = default)
    {
        var profile = await repository.GetByIdAsync(ProfileId, ct);
        if (profile is null)
        {
            profile = new Profile { Id = ProfileId };
            repository.Add(profile);
        }

        request.ApplyTo(profile);
        await repository.SaveChangesAsync(ct);
        return profile.ToDto();
    }
}
