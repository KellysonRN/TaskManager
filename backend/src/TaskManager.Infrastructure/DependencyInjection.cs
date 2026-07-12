using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Contracts;
using TaskManager.Infrastructure.Persistence;
using TaskManager.Infrastructure.Services;

namespace TaskManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<TaskManagerDbContext>(options =>
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                options.UseInMemoryDatabase("TaskManagerDb");
            }
            else
            {
                options.UseSqlite(connectionString);
            }
        });

        services.AddScoped<ITokenService, JwtTokenService>();

        return services;
    }
}
