using Microsoft.Extensions.DependencyInjection;
using VersionManager.Application.Softwares;
using VersionManager.Application.Versions;

namespace VersionManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ISoftwareService, SoftwareService>();
        services.AddScoped<IVersionService, VersionService>();
        return services;
    }
}
