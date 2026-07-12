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
}
