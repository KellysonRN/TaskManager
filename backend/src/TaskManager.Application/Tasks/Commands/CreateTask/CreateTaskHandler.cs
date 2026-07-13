using TaskManager.Application.Common.Contracts;
using TaskManager.Application.Common.Cqrs;
using TaskManager.Application.Common.Exceptions;
using TaskManager.Application.Tasks.Contracts;
using TaskManager.Application.Tasks.Dtos;
using TaskManager.Domain;

namespace TaskManager.Application.Tasks.Commands.CreateTask;

public class CreateTaskHandler : IRequestHandler<CreateTaskCommand, TaskDto>
{
    private static readonly string[] AllowedStatuses = ["Pending", "InProgress", "Completed"];

    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTaskHandler(ITaskRepository taskRepository, IUnitOfWork unitOfWork)
    {
        _taskRepository = taskRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TaskDto> HandleAsync(CreateTaskCommand request, CancellationToken cancellationToken = default)
    {
        ValidateRequest(request);

        var entity = new TaskEntity
        {
            Id = Guid.Empty,
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate,
            Status = request.Status,
            OwnerId = request.AuthenticatedUserId ?? Guid.Empty
        };

        var created = await _taskRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(created);
    }

    private static void ValidateRequest(CreateTaskCommand request)
    {
        if (string.IsNullOrWhiteSpace(request.Title)) throw new ValidationException("Title is required");
        if (request.Title != null && request.Title.Length > 200) throw new ValidationException("Title too long");
        if (request.Description != null && request.Description.Length > 1000) throw new ValidationException("Description too long");
        if (request.DueDate.HasValue && request.DueDate.Value < DateTime.UtcNow) throw new ValidationException("Due date cannot be in the past");

        if (!string.IsNullOrWhiteSpace(request.Status) && Array.IndexOf(AllowedStatuses, request.Status) < 0)
            throw new ValidationException("Invalid status");
    }

    private static TaskDto MapToDto(TaskEntity created)
    {
        return new TaskDto
        {
            Id = created.Id,
            Title = created.Title,
            Description = created.Description,
            DueDate = created.DueDate,
            Status = created.Status,
            OwnerId = created.OwnerId
        };
    }
}
