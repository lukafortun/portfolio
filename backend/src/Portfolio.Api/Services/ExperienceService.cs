using Portfolio.Api.DTOs;
using Portfolio.Api.Entities;
using Portfolio.Api.Mappings;
using Portfolio.Api.Repositories;

namespace Portfolio.Api.Services;

public class ExperienceService(IRepository<Experience> repository, ITagService tagService) : IExperienceService
{
    public async Task<IReadOnlyList<ExperienceDto>> GetAllAsync(CancellationToken ct = default)
    {
        var experiences = await repository.GetAllAsync(ct);
        return experiences
            // "YYYY-MM" strings sort like dates; "9999-99" puts ongoing entries (End = null) first.
            .OrderByDescending(x => x.End ?? "9999-99")
            .ThenByDescending(x => x.Start)
            .Select(x => x.ToDto())
            .ToList();
    }

    public async Task<ExperienceDto> CreateAsync(ExperienceRequest request, CancellationToken ct = default)
    {
        var experience = new Experience();
        request.ApplyTo(experience);
        await tagService.SyncAsync(experience.Tags, request.Tags, ct);
        repository.Add(experience);
        await repository.SaveChangesAsync(ct);
        return experience.ToDto();
    }

    public async Task<ExperienceDto?> UpdateAsync(int id, ExperienceRequest request, CancellationToken ct = default)
    {
        var experience = await repository.GetByIdAsync(id, ct);
        if (experience is null) return null;

        request.ApplyTo(experience);
        await tagService.SyncAsync(experience.Tags, request.Tags, ct);
        await repository.SaveChangesAsync(ct);
        await tagService.RemoveUnusedAsync(ct);
        return experience.ToDto();
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var experience = await repository.GetByIdAsync(id, ct);
        if (experience is null) return false;

        repository.Remove(experience);
        await repository.SaveChangesAsync(ct);
        await tagService.RemoveUnusedAsync(ct);
        return true;
    }
}
