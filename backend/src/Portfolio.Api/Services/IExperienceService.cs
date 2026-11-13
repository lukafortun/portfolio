using Portfolio.Api.DTOs;

namespace Portfolio.Api.Services;

public interface IExperienceService
{
    /// <returns>Most recent first: ongoing entries, then by end date, then by start date.</returns>
    Task<IReadOnlyList<ExperienceDto>> GetAllAsync(CancellationToken ct = default);

    Task<ExperienceDto> CreateAsync(ExperienceRequest request, CancellationToken ct = default);

    /// <returns>null if the entry does not exist.</returns>
    Task<ExperienceDto?> UpdateAsync(int id, ExperienceRequest request, CancellationToken ct = default);

    /// <returns>false if the entry does not exist.</returns>
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
