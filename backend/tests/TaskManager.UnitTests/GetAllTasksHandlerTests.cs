using Moq;
using Shouldly;
using Xunit;
using TaskManager.Application.Common.Cqrs;
using TaskManager.Domain;

namespace TaskManager.UnitTests.Application.Tasks;

public class GetAllTasksHandlerTests
{
    [Fact]
    public async Task GetAllTasks_Returns_All_Tasks()
    {
        var repositoryMock = new Mock<ITaskRepository>();
        repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TaskEntity>
            {
                new() { Id = Guid.NewGuid(), Title = "First task", Status = "Pending" },
                new() { Id = Guid.NewGuid(), Title = "Second task", Status = "Completed" }
            });

        var handler = new GetAllTasksHandler(repositoryMock.Object);

        var result = await handler.HandleAsync(new GetAllTasksQuery());

        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        result[0].Title.ShouldBe("First task");
        result[1].Status.ShouldBe("Completed");
        repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
