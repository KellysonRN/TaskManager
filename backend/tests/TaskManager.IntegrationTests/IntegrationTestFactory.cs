using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TaskManager.Infrastructure.Persistence;
using Xunit;

namespace TaskManager.IntegrationTests;

/// <summary>
/// Shared WebApplicationFactory used as an xUnit collection fixture.
/// ONE instance is created for the entire test run:
///   - schema is dropped and recreated once
///   - the admin user is seeded once
/// All test classes in [Collection("Integration")] share this instance.
/// </summary>
public sealed class IntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Prevent Program.cs DataSeeder from running — tests seed manually.
        builder.UseEnvironment("Testing");
    }

    public async Task InitializeAsync()
    {
        // Drop and recreate the schema once for all tests in the collection.
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TaskManagerDbContext>();
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();

        // Seed the single admin user used by every test class.
        var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
        await seeder.SeedAsync();
    }

    public new Task DisposeAsync() => Task.CompletedTask;
}

/// <summary>xUnit collection definition — ensures one shared factory.</summary>
[CollectionDefinition("Integration")]
public sealed class IntegrationCollection : ICollectionFixture<IntegrationTestFactory> { }

