using Shouldly;
using TaskManager.Domain;
using Xunit;

namespace TaskManager.UnitTests.Domain;

public class TaskEntityTests
{
    [Fact]
    public void TaskEntity_Defaults_Can_Be_Initialized()
    {
        var entity = new TaskEntity();

        entity.Id.ShouldBe(Guid.Empty);
        entity.Title.ShouldBeNull();
        entity.Description.ShouldBeNull();
        entity.DueDate.ShouldBeNull();
        entity.Status.ShouldBeNull();
        entity.OwnerId.ShouldBe(Guid.Empty);
    }
}
