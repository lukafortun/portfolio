using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Portfolio.Api.DTOs;

namespace Portfolio.Api.Tests.Integration;

public class UploadsTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    // Smallest valid PNG: a 1×1 transparent pixel.
    private static readonly byte[] Png = Convert.FromBase64String(
        "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNkYAAAAAYAAjCB0C8AAAAASUVORK5CYII=");

    private static MultipartFormDataContent Form(byte[] bytes, string fileName, string contentType = "image/png")
    {
        var file = new ByteArrayContent(bytes);
        file.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        return new MultipartFormDataContent { { file, "file", fileName } };
    }

    [Fact]
    public async Task Upload_when_anonymous_returns_401()
    {
        var response = await factory.CreateAnonymousClient().PostAsync("/api/uploads", Form(Png, "a.png"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Upload_when_not_admin_returns_403()
    {
        var client = factory.CreateUserClient("visiteur@test.local", "autre-groupe");

        var response = await client.PostAsync("/api/uploads", Form(Png, "a.png"));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Admin_upload_returns_a_public_url_serving_the_image()
    {
        var response = await factory.CreateUserClient().PostAsync("/api/uploads", Form(Png, "capture.png"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var upload = await response.Content.ReadFromJsonAsync<UploadResponse>();
        Assert.Matches(@"^/api/uploads/[0-9a-f]{32}\.png$", upload!.Url);

        // Served publicly, with the right type and forever cacheable.
        var image = await factory.CreateAnonymousClient().GetAsync(upload.Url);
        Assert.Equal(HttpStatusCode.OK, image.StatusCode);
        Assert.Equal("image/png", image.Content.Headers.ContentType?.MediaType);
        Assert.Contains("immutable", image.Headers.CacheControl?.ToString());
        Assert.Equal(Png, await image.Content.ReadAsByteArrayAsync());
    }

    [Fact]
    public async Task Upload_detects_the_real_format_not_the_file_name()
    {
        // A script renamed to .png, with an image Content-Type: still rejected.
        var fake = "<script>alert(1)</script>"u8.ToArray();

        var response = await factory.CreateUserClient().PostAsync("/api/uploads", Form(fake, "evil.png"));

        Assert.Equal(HttpStatusCode.UnsupportedMediaType, response.StatusCode);
    }

    [Fact]
    public async Task Upload_rejects_svg()
    {
        var svg = """<svg xmlns="http://www.w3.org/2000/svg"><script>alert(1)</script></svg>"""u8.ToArray();

        var response = await factory.CreateUserClient().PostAsync("/api/uploads", Form(svg, "logo.svg", "image/svg+xml"));

        Assert.Equal(HttpStatusCode.UnsupportedMediaType, response.StatusCode);
    }

    [Fact]
    public async Task Upload_rejects_images_over_the_size_limit()
    {
        // The factory sets Uploads:MaxBytes to 1024.
        var big = Png.Concat(new byte[2048]).ToArray();

        var response = await factory.CreateUserClient().PostAsync("/api/uploads", Form(big, "big.png"));

        Assert.Equal(HttpStatusCode.RequestEntityTooLarge, response.StatusCode);
    }

    [Fact]
    public async Task Upload_without_file_returns_400()
    {
        var response = await factory.CreateUserClient().PostAsync("/api/uploads", new MultipartFormDataContent());

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
