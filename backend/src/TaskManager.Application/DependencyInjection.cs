using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Auth.Commands.Login;
using TaskManager.Application.Tasks.Commands.CreateTask;
using TaskManager.Application.Tasks.Commands.DeleteTask;
using TaskManager.Application.Tasks.Commands.UpdateTask;
using TaskManager.Application.Tasks.Queries.GetAllTasks;

namespace TaskManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<CreateTaskHandler>();
        services.AddScoped<DeleteTaskHandler>();
        services.AddScoped<UpdateTaskHandler>();
        services.AddScoped<GetAllTasksHandler>();
        services.AddScoped<LoginHandler>();
        return services;
    }
}
