using TaskManager.Domain;

namespace TaskManager.Application.Auth.Contracts;

public interface IUserRepository
{
    Task<UserEntity?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserEntity> AddAsync(UserEntity user, CancellationToken cancellationToken = default);
}
