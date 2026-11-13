using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Data;
using Portfolio.Api.Repositories;
using Portfolio.Api.Services;

namespace Portfolio.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration config) =>
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("Default")));

    public static IServiceCollection AddRepositories(this IServiceCollection services) =>
        services
            .AddScoped<IProjectRepository, ProjectRepository>()
            // Open generic: IRepository<Profile> → Repository<Profile>, and so on for every entity.
            .AddScoped(typeof(IRepository<>), typeof(Repository<>));

    public static IServiceCollection AddApplicationServices(this IServiceCollection services) =>
        services
            .AddScoped<IProjectService, ProjectService>()
            .AddScoped<IProfileService, ProfileService>()
            .AddScoped<ISkillService, SkillService>()
            .AddScoped<IExperienceService, ExperienceService>();
}
