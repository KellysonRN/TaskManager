using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TaskManager.Application.Auth.Contracts;
using TaskManager.Domain;

namespace TaskManager.Infrastructure.Persistence;

public sealed class DataSeeder
{
    private readonly TaskManagerDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(TaskManagerDbContext dbContext, IPasswordHasher passwordHasher, ILogger<DataSeeder> logger)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        if (_dbContext.Users.Any())
        {
            _logger.LogInformation("Users table already has data. Skipping seed.");
            return;
        }

        var adminUser = new UserEntity
        {
            Id = Guid.NewGuid(),
            Email = "admin@taskmanager.dev",
            PasswordHash = _passwordHasher.Hash("Admin@123")
        };

        await _dbContext.Users.AddAsync(adminUser);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Seeded initial admin user: {Email}", adminUser.Email);
    }
}
