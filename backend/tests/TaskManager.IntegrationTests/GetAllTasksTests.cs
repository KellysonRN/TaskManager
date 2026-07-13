using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TaskManager.Application.Tasks.Dtos;
using TaskManager.Domain;
using TaskManager.Infrastructure.Persistence;
using Xunit;

namespace TaskManager.IntegrationTests;

public sealed class GetAllTasksTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public GetAllTasksTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllTasks_Returns_List_Of_Tasks()
    {
        await ResetDatabaseAsync();

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TaskManagerDbContext>();
        dbContext.Tasks.Add(new TaskEntity { Id = Guid.NewGuid(), Title = "Task 1", Status = "Pending" });
        dbContext.Tasks.Add(new TaskEntity { Id = Guid.NewGuid(), Title = "Task 2", Status = "Completed" });
        await dbContext.SaveChangesAsync();

        var response = await _client.GetAsync("/api/tasks");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var tasks = await response.Content.ReadFromJsonAsync<List<TaskDto>>();
        tasks.ShouldNotBeNull();
        tasks.Count.ShouldBe(2);
        tasks.Any(t => t.Title == "Task 1").ShouldBeTrue();
    }

    private async Task ResetDatabaseAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TaskManagerDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
        await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"Tasks\"");
        await dbContext.SaveChangesAsync();
    }
}
