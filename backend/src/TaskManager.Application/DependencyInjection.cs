using Microsoft.Extensions.DependencyInjection;

namespace TaskManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Application layer registrations remain explicit and minimal.
        // This is where command/query handlers and validation pipelines would be registered.
        return services;
    }
}
