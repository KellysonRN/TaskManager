using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application;
using Xunit;

namespace TaskManager.UnitTests;

public class DependencyInjectionTests
{
    [Fact]
    public void AddApplicationServices_ShouldNotThrow()
    {
        var services = new ServiceCollection();

        var exception = Record.Exception(() => services.AddApplicationServices());

        Assert.Null(exception);
    }
}
