using Microsoft.Extensions.Options;

namespace Portfolio.Api.Services;

public class UploadOptions
{
    public const string Section = "Uploads";

    /// <summary>Folder where images are stored. Relative paths are resolved from the content root.</summary>
    public string Path { get; set; } = "uploads";

    public long MaxBytes { get; set; } = 5 * 1024 * 1024;
}

public interface IImageStorage
{
    /// <summary>URL prefix under which stored images are served.</summary>
    const string PublicPath = "/api/uploads";

    /// <summary>Stores an image under a random name.</summary>
    /// <returns>Its public URL, or null if the content is not a supported image.</returns>
    Task<string?> SaveAsync(Stream content, CancellationToken ct = default);
}

public class ImageStorage(IOptions<UploadOptions> options) : IImageStorage
{
    public async Task<string?> SaveAsync(Stream content, CancellationToken ct = default)
    {
        // The real format is read from the first bytes: the file name and the Content-Type
        // sent by the browser can't be trusted.
        var header = new byte[12];
        var read = await content.ReadAtLeastAsync(header, header.Length, throwOnEndOfStream: false, ct);
        var extension = ImageFormats.DetectExtension(header.AsSpan(0, read));
        if (extension is null) return null;

        // Random name: no collisions, no path tricks ("../"), and URLs can be cached forever.
        var fileName = $"{Guid.NewGuid():N}{extension}";
        var path = System.IO.Path.Combine(options.Value.Path, fileName);

        await using (var file = File.Create(path))
        {
            await file.WriteAsync(header.AsMemory(0, read), ct);
            await content.CopyToAsync(file, ct);
        }

        return $"{IImageStorage.PublicPath}/{fileName}";
    }
}

/// <summary>Recognizes images from their "magic bytes". SVG is deliberately not supported: it can contain scripts.</summary>
public static class ImageFormats
{
    private static readonly byte[] Png = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]; // \x89PNG\r\n\x1A\n
    private static readonly byte[] Jpeg = [0xFF, 0xD8, 0xFF];

    public static string? DetectExtension(ReadOnlySpan<byte> header)
    {
        if (header.StartsWith(Png)) return ".png";
        if (header.StartsWith(Jpeg)) return ".jpg";
        if (header.StartsWith("GIF87a"u8) || header.StartsWith("GIF89a"u8)) return ".gif";
        if (header.Length >= 12 && header.StartsWith("RIFF"u8) && header[8..12].SequenceEqual("WEBP"u8)) return ".webp";
        return null;
    }
}
