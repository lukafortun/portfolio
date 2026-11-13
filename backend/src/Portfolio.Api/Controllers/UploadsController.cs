using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Portfolio.Api.DTOs;
using Portfolio.Api.Extensions;
using Portfolio.Api.Services;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("api/uploads")]
[Authorize(Policy = AuthSetup.AdminPolicy)]
public class UploadsController(IImageStorage storage, IOptions<UploadOptions> options) : ControllerBase
{
    // Hard limit for the whole request (Kestrel rejects bigger bodies before they are read).
    // The configured MaxBytes, checked below, is the real per-image limit.
    private const long RequestLimit = 20 * 1024 * 1024;

    /// <summary>Uploads a PNG, JPEG, GIF or WebP image (multipart/form-data, field "file").</summary>
    [HttpPost]
    [RequestSizeLimit(RequestLimit)]
    [RequestFormLimits(MultipartBodyLengthLimit = RequestLimit)]
    public async Task<ActionResult<UploadResponse>> Upload(IFormFile file, CancellationToken ct)
    {
        var maxBytes = options.Value.MaxBytes;
        if (file.Length > maxBytes)
        {
            return Problem(
                statusCode: StatusCodes.Status413PayloadTooLarge,
                title: "Image too large",
                detail: $"Images are limited to {maxBytes / 1024 / 1024} MB.");
        }

        await using var stream = file.OpenReadStream();
        var url = await storage.SaveAsync(stream, ct);
        if (url is null)
        {
            return Problem(
                statusCode: StatusCodes.Status415UnsupportedMediaType,
                title: "Unsupported image",
                detail: "Only PNG, JPEG, GIF and WebP images are accepted.");
        }

        return Created(url, new UploadResponse(url));
    }
}
