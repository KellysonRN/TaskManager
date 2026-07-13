using TaskManager.Application.Common.Contracts;
using TaskManager.Application.Common.Cqrs;
using TaskManager.Application.Common.Exceptions;
using TaskManager.Application.Tasks.Contracts;

namespace TaskManager.Application.Tasks.Commands.DeleteTask;

public sealed class DeleteTaskHandler : IRequestHandler<DeleteTaskCommand, bool>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTaskHandler(ITaskRepository taskRepository, IUnitOfWork unitOfWork)
    {
        _taskRepository = taskRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> HandleAsync(DeleteTaskCommand request, CancellationToken cancellationToken = default)
    {
        if (request.Id == Guid.Empty)
        {
            throw new ValidationException("Task id is required");
        }

        var deleted = await _taskRepository.DeleteByIdAsync(request.Id, cancellationToken);
        if (!deleted)
        {
            throw new NotFoundException("Task not found");
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
