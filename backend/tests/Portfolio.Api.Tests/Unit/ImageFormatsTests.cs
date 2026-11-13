using System.Text;
using Portfolio.Api.Services;

namespace Portfolio.Api.Tests.Unit;

public class ImageFormatsTests
{
    [Theory]
    [InlineData(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0, 0, 0, 0 }, ".png")]
    [InlineData(new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0, 0, 0, 0, 0, 0, 0, 0 }, ".jpg")]
    public void Detects_binary_signatures(byte[] header, string expected) =>
        Assert.Equal(expected, ImageFormats.DetectExtension(header));

    [Theory]
    [InlineData("GIF89a\0\0\0\0\0\0", ".gif")]
    [InlineData("GIF87a\0\0\0\0\0\0", ".gif")]
    [InlineData("RIFF\0\0\0\0WEBP", ".webp")]
    public void Detects_text_signatures(string header, string expected) =>
        Assert.Equal(expected, ImageFormats.DetectExtension(Encoding.ASCII.GetBytes(header)));

    [Theory]
    [InlineData("<svg xmlns=…")] // SVG can contain scripts
    [InlineData("RIFF\0\0\0\0WAVE")] // RIFF, but audio
    [InlineData("%PDF-1.7")]
    [InlineData("")]
    public void Rejects_anything_else(string header) =>
        Assert.Null(ImageFormats.DetectExtension(Encoding.UTF8.GetBytes(header)));
}
