using System.Threading;
using System.Threading.Tasks;
using TaskManager.Application.Common.Cqrs;

namespace TaskManager.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly TaskManagerDbContext _dbContext;

    public UnitOfWork(TaskManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
