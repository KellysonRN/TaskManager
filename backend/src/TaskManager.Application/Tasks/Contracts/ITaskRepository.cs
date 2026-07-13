using TaskManager.Domain;

namespace TaskManager.Application.Tasks.Contracts;

public interface ITaskRepository
{
    Task<TaskEntity> AddAsync(TaskEntity entity, CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<TaskEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<TaskEntity?> UpdateAsync(
        Guid id,
        string? title,
        string? description,
        DateTime? dueDate,
        string? status,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
