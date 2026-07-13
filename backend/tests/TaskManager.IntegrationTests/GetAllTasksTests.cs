using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TaskManager.Application.Tasks.Dtos;
using TaskManager.Domain;
using TaskManager.Infrastructure.Persistence;
using Xunit;

namespace TaskManager.IntegrationTests;

[Collection("Integration")]
public sealed class GetAllTasksTests
{
    private readonly HttpClient _client;
    private readonly IntegrationTestFactory _factory;

    public GetAllTasksTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        SetBearerTokenAsync().GetAwaiter().GetResult();
    }

    private async Task SetBearerTokenAsync()
    {
        var res = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "admin@taskmanager.dev",
            password = "Admin@123"
        });
        var body = await res.Content.ReadFromJsonAsync<TokenResponse>();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", body!.Token);
    }

    [Fact]
    public async Task GetAllTasks_Returns_List_Of_Tasks()
    {
        await ResetTasksAsync();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TaskManagerDbContext>();
        db.Tasks.Add(new TaskEntity { Id = Guid.NewGuid(), Title = "Task 1", Status = "Pending" });
        db.Tasks.Add(new TaskEntity { Id = Guid.NewGuid(), Title = "Task 2", Status = "Completed" });
        await db.SaveChangesAsync();

        var response = await _client.GetAsync("/api/tasks");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var tasks = await response.Content.ReadFromJsonAsync<List<TaskDto>>();
        tasks.ShouldNotBeNull();
        tasks.Count.ShouldBe(2);
        tasks.Any(t => t.Title == "Task 1").ShouldBeTrue();
    }

    [Fact]
    public async Task GetAllTasks_WithoutToken_Returns401()
    {
        var unauthClient = _factory.CreateClient();
        var response = await unauthClient.GetAsync("/api/tasks");
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    private async Task ResetTasksAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TaskManagerDbContext>();
        db.Tasks.RemoveRange(db.Tasks);
        await db.SaveChangesAsync();
    }

    private sealed record TokenResponse(string Token);
}
