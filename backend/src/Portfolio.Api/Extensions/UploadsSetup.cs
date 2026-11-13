using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Portfolio.Api.Services;

namespace Portfolio.Api.Extensions;

public static class UploadsSetup
{
    public static IServiceCollection AddUploads(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<UploadOptions>()
            .Bind(config.GetSection(UploadOptions.Section))
            .PostConfigure<IWebHostEnvironment>((o, env) =>
                o.Path = Path.GetFullPath(o.Path, env.ContentRootPath));

        return services.AddSingleton<IImageStorage, ImageStorage>();
    }

    /// <summary>Serves stored images at /api/uploads/{file}, publicly.</summary>
    public static WebApplication UseUploads(this WebApplication app)
    {
        var path = app.Services.GetRequiredService<IOptions<UploadOptions>>().Value.Path;
        Directory.CreateDirectory(path);

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(path),
            RequestPath = IImageStorage.PublicPath,
            OnPrepareResponse = ctx =>
            {
                var headers = ctx.Context.Response.Headers;
                // File names are random and never reused, so they can be cached forever.
                headers.CacheControl = "public, max-age=31536000, immutable";
                // Never let the browser guess another type (e.g. HTML) from the content.
                headers.XContentTypeOptions = "nosniff";
            },
        });

        return app;
    }
}
