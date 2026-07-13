using TaskManager.Application.Common.Cqrs;
using TaskManager.Application.Tasks.Dtos;

namespace TaskManager.Application.Tasks.Queries.GetAllTasks;

public class GetAllTasksQuery : IRequest<IReadOnlyList<TaskDto>>
{
}
