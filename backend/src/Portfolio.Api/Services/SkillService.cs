using Portfolio.Api.DTOs;
using Portfolio.Api.Entities;
using Portfolio.Api.Mappings;
using Portfolio.Api.Repositories;

namespace Portfolio.Api.Services;

public class SkillService(IRepository<SkillGroup> repository) : ISkillService
{
    public async Task<IReadOnlyList<SkillGroupDto>> GetAllAsync(CancellationToken ct = default)
    {
        var groups = await repository.GetAllAsync(ct);
        return groups
            .OrderBy(g => g.SortOrder)
            .ThenBy(g => g.Name)
            .Select(g => g.ToDto())
            .ToList();
    }

    public async Task<SkillGroupDto> CreateAsync(SkillGroupRequest request, CancellationToken ct = default)
    {
        var group = new SkillGroup();
        request.ApplyTo(group);
        repository.Add(group);
        await repository.SaveChangesAsync(ct);
        return group.ToDto();
    }

    public async Task<SkillGroupDto?> UpdateAsync(int id, SkillGroupRequest request, CancellationToken ct = default)
    {
        var group = await repository.GetByIdAsync(id, ct);
        if (group is null) return null;

        request.ApplyTo(group);
        await repository.SaveChangesAsync(ct);
        return group.ToDto();
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var group = await repository.GetByIdAsync(id, ct);
        if (group is null) return false;

        repository.Remove(group);
        await repository.SaveChangesAsync(ct);
        return true;
    }
}
