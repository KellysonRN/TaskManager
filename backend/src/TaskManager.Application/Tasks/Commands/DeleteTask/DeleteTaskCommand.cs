using TaskManager.Application.Common.Cqrs;

namespace TaskManager.Application.Tasks.Commands.DeleteTask;

public sealed class DeleteTaskCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}
