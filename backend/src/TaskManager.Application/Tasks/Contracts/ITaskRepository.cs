using TaskManager.Domain;

namespace TaskManager.Application.Tasks.Contracts;

public interface ITaskRepository
{
    Task<TaskEntity> AddAsync(TaskEntity entity, CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<TaskEntity>> GetAllAsync(CancellationToken cancellationToken = default);
}
