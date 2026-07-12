using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Common.Cqrs;

namespace TaskManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<CreateTaskHandler>();
        services.AddScoped<GetAllTasksHandler>();
        return services;
    }
}
