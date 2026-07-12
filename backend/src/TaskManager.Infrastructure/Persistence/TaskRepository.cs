using System;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.Application.Common.Cqrs;
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
}
