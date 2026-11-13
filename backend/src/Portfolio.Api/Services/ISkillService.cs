using Portfolio.Api.DTOs;

namespace Portfolio.Api.Services;

public interface ISkillService
{
    /// <returns>Groups sorted by SortOrder, then name.</returns>
    Task<IReadOnlyList<SkillGroupDto>> GetAllAsync(CancellationToken ct = default);

    Task<SkillGroupDto> CreateAsync(SkillGroupRequest request, CancellationToken ct = default);

    /// <returns>null if the group does not exist.</returns>
    Task<SkillGroupDto?> UpdateAsync(int id, SkillGroupRequest request, CancellationToken ct = default);

    /// <returns>false if the group does not exist.</returns>
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
