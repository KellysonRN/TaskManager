using System.Threading;
using System.Threading.Tasks;
using TaskManager.Domain;

namespace TaskManager.Application.Common.Cqrs;

public interface ITaskRepository
{
    Task<TaskEntity> AddAsync(TaskEntity entity, CancellationToken cancellationToken = default);
}
