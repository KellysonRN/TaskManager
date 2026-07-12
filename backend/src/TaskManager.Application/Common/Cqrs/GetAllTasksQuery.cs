namespace TaskManager.Application.Common.Cqrs;

public class GetAllTasksQuery : IRequest<IReadOnlyList<TaskDto>>
{
}
