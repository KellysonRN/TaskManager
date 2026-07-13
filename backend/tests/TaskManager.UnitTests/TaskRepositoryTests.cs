using Microsoft.EntityFrameworkCore;
using Shouldly;
using TaskManager.Domain;
using TaskManager.Infrastructure.Persistence;
using Xunit;

namespace TaskManager.UnitTests.Infrastructure.Persistence;

public class TaskRepositoryTests
{
    [Fact]
    public async Task AddAsync_Persists_Task_And_Returns_It()
    {
        var options = new DbContextOptionsBuilder<TaskManagerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new TaskManagerDbContext(options);
        var repository = new TaskRepository(context);

        var entity = new TaskEntity
        {
            Title = "Repository task",
            Description = "Persisted through repository",
            DueDate = DateTime.UtcNow.AddDays(1),
            Status = "Pending",
            OwnerId = Guid.NewGuid()
        };

        var created = await repository.AddAsync(entity);

        await context.SaveChangesAsync();

        created.Id.ShouldNotBe(Guid.Empty);
        context.Tasks.Count().ShouldBe(1);
        context.Tasks.Single().Title.ShouldBe("Repository task");
    }

    [Fact]
    public async Task DeleteByIdAsync_Removes_Task_When_Found()
    {
        var options = new DbContextOptionsBuilder<TaskManagerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new TaskManagerDbContext(options);
        var task = new TaskEntity
        {
            Id = Guid.NewGuid(),
            Title = "Delete me",
            OwnerId = Guid.NewGuid()
        };
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var repository = new TaskRepository(context);
        var deleted = await repository.DeleteByIdAsync(task.Id);
        await context.SaveChangesAsync();

        deleted.ShouldBeTrue();
        context.Tasks.ShouldBeEmpty();
    }

    [Fact]
    public async Task DeleteByIdAsync_ReturnsFalse_When_Task_DoesNotExist()
    {
        var options = new DbContextOptionsBuilder<TaskManagerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new TaskManagerDbContext(options);
        var repository = new TaskRepository(context);

        var deleted = await repository.DeleteByIdAsync(Guid.NewGuid());

        deleted.ShouldBeFalse();
    }

    [Fact]
    public async Task UpdateAsync_Updates_Task_When_Found()
    {
        var options = new DbContextOptionsBuilder<TaskManagerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new TaskManagerDbContext(options);
        var task = new TaskEntity
        {
            Id = Guid.NewGuid(),
            Title = "Original",
            Description = "Original desc",
            Status = "Pending",
            OwnerId = Guid.NewGuid()
        };
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var repository = new TaskRepository(context);
        var dueDate = DateTime.UtcNow.AddDays(2);
        var updated = await repository.UpdateAsync(task.Id, "Updated", "Updated desc", dueDate, "InProgress");
        await context.SaveChangesAsync();

        updated.ShouldNotBeNull();
        updated.Title.ShouldBe("Updated");
        updated.Description.ShouldBe("Updated desc");
        updated.Status.ShouldBe("InProgress");
        updated.DueDate.ShouldBe(dueDate);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_When_Task_DoesNotExist()
    {
        var options = new DbContextOptionsBuilder<TaskManagerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new TaskManagerDbContext(options);
        var repository = new TaskRepository(context);

        var updated = await repository.UpdateAsync(Guid.NewGuid(), "Updated", "Updated", DateTime.UtcNow.AddDays(1), "Pending");

        updated.ShouldBeNull();
    }
}
