using System.ComponentModel.DataAnnotations;

namespace Portfolio.Api.DTOs;

public record ProjectDto(int Id, string Title, string Description, string? RepoUrl, string? LiveUrl, DateTime CreatedAt);

public record ProjectRequest(
    [Required, StringLength(200)] string Title,
    [Required, StringLength(4000)] string Description,
    [Url, StringLength(500)] string? RepoUrl,
    [Url, StringLength(500)] string? LiveUrl);
