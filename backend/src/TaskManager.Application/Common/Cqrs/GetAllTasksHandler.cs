namespace TaskManager.Application.Common.Cqrs;

public class GetAllTasksHandler : IRequestHandler<GetAllTasksQuery, IReadOnlyList<TaskDto>>
{
    private readonly ITaskRepository _taskRepository;

    public GetAllTasksHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<IReadOnlyList<TaskDto>> HandleAsync(GetAllTasksQuery request, CancellationToken cancellationToken = default)
    {
        var tasks = await _taskRepository.GetAllAsync(cancellationToken);
        return tasks.Select(MapToDto).ToList();
    }

    private static TaskDto MapToDto(Domain.TaskEntity task)
    {
        return new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            Status = task.Status,
            OwnerId = task.OwnerId
        };
    }
}
