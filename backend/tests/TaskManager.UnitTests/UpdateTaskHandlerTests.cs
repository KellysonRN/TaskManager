using Moq;
using Shouldly;
using TaskManager.Application.Common.Contracts;
using TaskManager.Application.Common.Exceptions;
using TaskManager.Application.Tasks.Commands.UpdateTask;
using TaskManager.Application.Tasks.Contracts;
using TaskManager.Domain;
using Xunit;

namespace TaskManager.UnitTests.Application.Tasks;

public class UpdateTaskHandlerTests
{
    [Fact]
    public async Task UpdateTask_Success()
    {
        // Purpose: verify happy path updates task and returns mapped DTO.
        var taskId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var repositoryMock = new Mock<ITaskRepository>();
        repositoryMock
            .Setup(r => r.UpdateAsync(taskId, "Updated", "Updated desc", It.IsAny<DateTime?>(), "InProgress", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TaskEntity
            {
                Id = taskId,
                Title = "Updated",
                Description = "Updated desc",
                DueDate = DateTime.UtcNow.AddDays(1),
                Status = "InProgress",
                OwnerId = ownerId
            });

        var uowMock = new Mock<IUnitOfWork>();
        uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new UpdateTaskHandler(repositoryMock.Object, uowMock.Object);
        var command = new UpdateTaskCommand
        {
            Id = taskId,
            Title = "Updated",
            Description = "Updated desc",
            DueDate = DateTime.UtcNow.AddDays(1),
            Status = "InProgress"
        };

        var result = await handler.HandleAsync(command);

        result.Id.ShouldBe(taskId);
        result.Title.ShouldBe("Updated");
        result.Status.ShouldBe("InProgress");
        result.OwnerId.ShouldBe(ownerId);
        uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTask_Fails_WhenIdIsEmpty()
    {
        // Purpose: invalid identifier must be rejected immediately.
        var repositoryMock = new Mock<ITaskRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        var handler = new UpdateTaskHandler(repositoryMock.Object, uowMock.Object);

        await Should.ThrowAsync<ValidationException>(() =>
            handler.HandleAsync(new UpdateTaskCommand { Id = Guid.Empty, Title = "T" }));
    }

    [Fact]
    public async Task UpdateTask_Fails_WhenTitleIsEmpty()
    {
        // Purpose: title remains a required field for updates.
        var repositoryMock = new Mock<ITaskRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        var handler = new UpdateTaskHandler(repositoryMock.Object, uowMock.Object);

        await Should.ThrowAsync<ValidationException>(() =>
            handler.HandleAsync(new UpdateTaskCommand { Id = Guid.NewGuid(), Title = "" }));
    }

    [Fact]
    public async Task UpdateTask_Fails_WhenTaskNotFound()
    {
        // Purpose: updating missing task should surface not-found error.
        var repositoryMock = new Mock<ITaskRepository>();
        repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskEntity?)null);

        var uowMock = new Mock<IUnitOfWork>();
        var handler = new UpdateTaskHandler(repositoryMock.Object, uowMock.Object);

        await Should.ThrowAsync<NotFoundException>(() =>
            handler.HandleAsync(new UpdateTaskCommand { Id = Guid.NewGuid(), Title = "Updated" }));

        uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
