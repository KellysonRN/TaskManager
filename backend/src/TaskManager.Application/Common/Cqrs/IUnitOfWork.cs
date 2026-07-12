using System.Threading;
using System.Threading.Tasks;

namespace TaskManager.Application.Common.Cqrs;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
