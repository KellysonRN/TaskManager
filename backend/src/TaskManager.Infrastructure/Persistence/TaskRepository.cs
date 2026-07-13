using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Tasks.Contracts;
using TaskManager.Domain;

namespace TaskManager.Infrastructure.Persistence;

public sealed class TaskRepository : ITaskRepository
{
    private readonly TaskManagerDbContext _dbContext;

    public TaskRepository(TaskManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TaskEntity> AddAsync(TaskEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity.Id == Guid.Empty)
        {
            entity.Id = Guid.NewGuid();
        }

        await _dbContext.Tasks.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task<IReadOnlyList<TaskEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Tasks.OrderByDescending(t => t.DueDate ?? DateTime.MaxValue).ThenBy(t => t.Title).ToListAsync(cancellationToken);
    }

    public async Task<TaskEntity?> UpdateAsync(
        Guid id,
        string? title,
        string? description,
        DateTime? dueDate,
        string? status,
        CancellationToken cancellationToken = default)
    {
        var task = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (task is null)
        {
            return null;
        }

        task.Title = title;
        task.Description = description;
        task.DueDate = dueDate;
        task.Status = status;

        return task;
    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var task = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (task is null)
        {
            return false;
        }

        _dbContext.Tasks.Remove(task);
        return true;
    }
}
