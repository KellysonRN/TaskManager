using TaskManager.Application.Common.Cqrs;
using TaskManager.Application.Tasks.Dtos;

namespace TaskManager.Application.Tasks.Commands.UpdateTask;

public sealed class UpdateTaskCommand : IRequest<TaskDto>
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Status { get; set; }
}
