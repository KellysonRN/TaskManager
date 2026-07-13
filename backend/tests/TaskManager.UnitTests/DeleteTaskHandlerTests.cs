using Moq;
using Shouldly;
using TaskManager.Application.Common.Contracts;
using TaskManager.Application.Common.Exceptions;
using TaskManager.Application.Tasks.Commands.DeleteTask;
using TaskManager.Application.Tasks.Contracts;
using Xunit;

namespace TaskManager.UnitTests.Application.Tasks;

public class DeleteTaskHandlerTests
{
    [Fact]
    public async Task DeleteTask_Success()
    {
        // Purpose: verify happy path deletes a task and commits once.
        var taskId = Guid.NewGuid();
        var repositoryMock = new Mock<ITaskRepository>();
        repositoryMock
            .Setup(r => r.DeleteByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var uowMock = new Mock<IUnitOfWork>();
        uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new DeleteTaskHandler(repositoryMock.Object, uowMock.Object);

        var result = await handler.HandleAsync(new DeleteTaskCommand { Id = taskId });

        result.ShouldBeTrue();
        repositoryMock.Verify(r => r.DeleteByIdAsync(taskId, It.IsAny<CancellationToken>()), Times.Once);
        uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteTask_Fails_WhenIdIsEmpty()
    {
        // Purpose: prevent invalid requests from reaching persistence.
        var repositoryMock = new Mock<ITaskRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        var handler = new DeleteTaskHandler(repositoryMock.Object, uowMock.Object);

        await Should.ThrowAsync<ValidationException>(() =>
            handler.HandleAsync(new DeleteTaskCommand { Id = Guid.Empty }));

        repositoryMock.Verify(r => r.DeleteByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteTask_Fails_WhenTaskNotFound()
    {
        // Purpose: return a 404-style domain error when id does not exist.
        var taskId = Guid.NewGuid();
        var repositoryMock = new Mock<ITaskRepository>();
        repositoryMock
            .Setup(r => r.DeleteByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var uowMock = new Mock<IUnitOfWork>();
        var handler = new DeleteTaskHandler(repositoryMock.Object, uowMock.Object);

        await Should.ThrowAsync<NotFoundException>(() =>
            handler.HandleAsync(new DeleteTaskCommand { Id = taskId }));

        uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
