using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VersionManager.Application.Abstractions;
using VersionManager.Infrastructure.Persistence;
using VersionManager.Infrastructure.Repositories;

namespace VersionManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlServer(config.GetConnectionString("Default")));

        services.AddScoped<ISoftwareRepository, SoftwareRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
