using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Auth.Contracts;
using TaskManager.Domain;

namespace TaskManager.Infrastructure.Persistence;

public sealed class UserRepository : IUserRepository
{
    private readonly TaskManagerDbContext _dbContext;

    public UserRepository(TaskManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserEntity?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<UserEntity> AddAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        if (user.Id == Guid.Empty)
            user.Id = Guid.NewGuid();

        await _dbContext.Users.AddAsync(user, cancellationToken);
        return user;
    }
}
