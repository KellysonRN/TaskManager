using Moq;
using Shouldly;
using Xunit;
using TaskManager.Application.Common.Cqrs;
using TaskManager.Domain;

namespace TaskManager.UnitTests.Application.Tasks;

public class CreateTaskHandlerTests
{
    [Fact]
    public async Task CreateTask_Success()
    {
        // Purpose: verify happy path creates a task and returns the created DTO.
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = "New Task",
            Description = "A test task",
            DueDate = DateTime.UtcNow.AddDays(1),
            Status = "Pending"
        };

        var repositoryMock = new Mock<ITaskRepository>();
        repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskEntity e, CancellationToken _) => { e.Id = Guid.NewGuid(); return e; });

        var uowMock = new Mock<IUnitOfWork>();
        uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new CreateTaskHandler(repositoryMock.Object, uowMock.Object);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(Guid.Empty);
        result.Title.ShouldBe(command.Title);
        repositoryMock.Verify(r => r.AddAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateTask_Fails_WhenTitleEmpty()
    {
        // Purpose: Title is required.
        var command = new CreateTaskCommand { Title = string.Empty };

        var repositoryMock = new Mock<ITaskRepository>();
        var uowMock = new Mock<IUnitOfWork>();

        var handler = new CreateTaskHandler(repositoryMock.Object, uowMock.Object);

        await Should.ThrowAsync<ValidationException>(async () => await handler.HandleAsync(command));
    }

    [Fact]
    public async Task CreateTask_Fails_WhenTitleTooLong()
    {
        // Purpose: Title max length 200 chars.
        var command = new CreateTaskCommand { Title = new string('A', 201) };

        var repositoryMock = new Mock<ITaskRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        var handler = new CreateTaskHandler(repositoryMock.Object, uowMock.Object);

        await Should.ThrowAsync<ValidationException>(async () => await handler.HandleAsync(command));
    }

    [Fact]
    public async Task CreateTask_Fails_WhenDescriptionTooLong()
    {
        // Purpose: Description max length 1000 chars.
        var command = new CreateTaskCommand { Title = "T", Description = new string('D', 1001) };

        var repositoryMock = new Mock<ITaskRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        var handler = new CreateTaskHandler(repositoryMock.Object, uowMock.Object);

        await Should.ThrowAsync<ValidationException>(async () => await handler.HandleAsync(command));
    }

    [Fact]
    public async Task CreateTask_Fails_WhenDueDateInPast()
    {
        // Purpose: DueDate cannot be in the past.
        var command = new CreateTaskCommand { Title = "T", DueDate = DateTime.UtcNow.AddDays(-1) };

        var repositoryMock = new Mock<ITaskRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        var handler = new CreateTaskHandler(repositoryMock.Object, uowMock.Object);

        await Should.ThrowAsync<ValidationException>(async () => await handler.HandleAsync(command));
    }

    [Fact]
    public async Task CreateTask_Fails_WhenStatusInvalid()
    {
        // Purpose: Status must be Pending, InProgress or Completed.
        var command = new CreateTaskCommand { Title = "T", Status = "Unknown" };

        var repositoryMock = new Mock<ITaskRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        var handler = new CreateTaskHandler(repositoryMock.Object, uowMock.Object);

        await Should.ThrowAsync<ValidationException>(async () => await handler.HandleAsync(command));
    }

    [Fact]
    public async Task Repository_AddAsync_Called_Once()
    {
        // Purpose: ensure repository is called exactly once.
        var command = new CreateTaskCommand { Title = "T", DueDate = DateTime.UtcNow.AddDays(1) };
        var repositoryMock = new Mock<ITaskRepository>();
        repositoryMock.Setup(r => r.AddAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>())).ReturnsAsync((TaskEntity e, CancellationToken _) => { e.Id = Guid.NewGuid(); return e; });
        var uowMock = new Mock<IUnitOfWork>();
        uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new CreateTaskHandler(repositoryMock.Object, uowMock.Object);
        await handler.HandleAsync(command);

        repositoryMock.Verify(r => r.AddAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UnitOfWork_SaveChanges_Called_Once()
    {
        // Purpose: ensure unit of work is committed once.
        var command = new CreateTaskCommand { Title = "T", DueDate = DateTime.UtcNow.AddDays(1) };
        var repositoryMock = new Mock<ITaskRepository>();
        repositoryMock.Setup(r => r.AddAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>())).ReturnsAsync((TaskEntity e, CancellationToken _) => { e.Id = Guid.NewGuid(); return e; });
        var uowMock = new Mock<IUnitOfWork>();
        uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new CreateTaskHandler(repositoryMock.Object, uowMock.Object);
        await handler.HandleAsync(command);

        uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreatedEntity_Contains_AuthenticatedUserId()
    {
        // Purpose: Owner of the task must be the authenticated user.
        var userId = Guid.NewGuid();
        var command = new CreateTaskCommand { Title = "T", DueDate = DateTime.UtcNow.AddDays(1), AuthenticatedUserId = userId };

        TaskEntity captured = null!;
        var repositoryMock = new Mock<ITaskRepository>();
        repositoryMock.Setup(r => r.AddAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskEntity e, CancellationToken _) => { captured = e; e.Id = Guid.NewGuid(); return e; });

        var uowMock = new Mock<IUnitOfWork>();
        uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new CreateTaskHandler(repositoryMock.Object, uowMock.Object);
        var result = await handler.HandleAsync(command);

        captured.ShouldNotBeNull();
        captured.OwnerId.ShouldBe(userId);
        result.OwnerId.ShouldBe(userId);
    }

    [Fact]
    public async Task ResponseDto_Is_Correctly_Mapped()
    {
        // Purpose: ensure handler returns properly mapped DTO.
        var userId = Guid.NewGuid();
        var command = new CreateTaskCommand { Title = "T", Description = "D", DueDate = DateTime.UtcNow.AddDays(1), Status = "Pending", AuthenticatedUserId = userId };

        var repositoryMock = new Mock<ITaskRepository>();
        repositoryMock.Setup(r => r.AddAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskEntity e, CancellationToken _) => { e.Id = Guid.NewGuid(); return e; });
        var uowMock = new Mock<IUnitOfWork>();
        uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new CreateTaskHandler(repositoryMock.Object, uowMock.Object);
        var dto = await handler.HandleAsync(command);

        dto.ShouldNotBeNull();
        dto.Id.ShouldNotBe(Guid.Empty);
        dto.Title.ShouldBe(command.Title);
        dto.Description.ShouldBe(command.Description);
        dto.DueDate.ShouldBe(command.DueDate);
        dto.Status.ShouldBe(command.Status);
        dto.OwnerId.ShouldBe(userId);
    }
}
