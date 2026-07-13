using TaskManager.Application.Common.Contracts;
using TaskManager.Application.Common.Cqrs;
using TaskManager.Application.Common.Exceptions;
using TaskManager.Application.Tasks.Contracts;
using TaskManager.Application.Tasks.Dtos;

namespace TaskManager.Application.Tasks.Commands.UpdateTask;

public sealed class UpdateTaskHandler : IRequestHandler<UpdateTaskCommand, TaskDto>
{
    private static readonly string[] AllowedStatuses = ["Pending", "InProgress", "Completed"];

    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTaskHandler(ITaskRepository taskRepository, IUnitOfWork unitOfWork)
    {
        _taskRepository = taskRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TaskDto> HandleAsync(UpdateTaskCommand request, CancellationToken cancellationToken = default)
    {
        ValidateRequest(request);

        var updated = await _taskRepository.UpdateAsync(request.Id, request.Title, request.Description, request.DueDate, request.Status, cancellationToken);
        if (updated is null)
        {
            throw new NotFoundException("Task not found");
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new TaskDto
        {
            Id = updated.Id,
            Title = updated.Title,
            Description = updated.Description,
            DueDate = updated.DueDate,
            Status = updated.Status,
            OwnerId = updated.OwnerId
        };
    }

    private static void ValidateRequest(UpdateTaskCommand request)
    {
        if (request.Id == Guid.Empty) throw new ValidationException("Task id is required");
        if (string.IsNullOrWhiteSpace(request.Title)) throw new ValidationException("Title is required");
        if (request.Title != null && request.Title.Length > 200) throw new ValidationException("Title too long");
        if (request.Description != null && request.Description.Length > 1000) throw new ValidationException("Description too long");
        if (request.DueDate.HasValue && request.DueDate.Value < DateTime.UtcNow) throw new ValidationException("Due date cannot be in the past");
        if (!string.IsNullOrWhiteSpace(request.Status) && Array.IndexOf(AllowedStatuses, request.Status) < 0)
            throw new ValidationException("Invalid status");
    }
}
