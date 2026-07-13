using Microsoft.EntityFrameworkCore;
using TaskManager.Domain;

namespace TaskManager.Infrastructure.Persistence;

public sealed class TaskManagerDbContext : DbContext
{
    public TaskManagerDbContext(DbContextOptions<TaskManagerDbContext> options)
        : base(options)
    {
    }

    public DbSet<TaskEntity> Tasks => Set<TaskEntity>();
    public DbSet<UserEntity> Users => Set<UserEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Title).HasMaxLength(200);
            entity.Property(x => x.Description).HasMaxLength(1000);
        });

        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Email).HasMaxLength(256).IsRequired();
            entity.Property(x => x.PasswordHash).IsRequired();
            entity.HasIndex(x => x.Email).IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }
}
