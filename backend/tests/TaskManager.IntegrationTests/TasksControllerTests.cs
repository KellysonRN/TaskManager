using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TaskManager.Application.Common.Cqrs;
using TaskManager.Infrastructure.Persistence;
using Xunit;

namespace TaskManager.IntegrationTests;

public sealed class TasksControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public TasksControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateTask_Returns_Created_Status_And_Created_Task()
    {
        await ResetDatabaseAsync();

        var payload = new
        {
            title = "New task",
            description = "A task created through the API",
            dueDate = DateTime.UtcNow.AddDays(1),
            status = "Pending"
        };

        var response = await _client.PostAsJsonAsync("/api/tasks", payload);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        var createdTask = await response.Content.ReadFromJsonAsync<TaskDto>();
        createdTask.ShouldNotBeNull();
        createdTask.Title.ShouldBe("New task");
        createdTask.Status.ShouldBe("Pending");
        createdTask.Id.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public async Task CreateTask_Returns_BadRequest_For_Invalid_Title()
    {
        await ResetDatabaseAsync();

        var payload = new
        {
            title = string.Empty,
            description = "Missing title",
            dueDate = DateTime.UtcNow.AddDays(1),
            status = "Pending"
        };

        var response = await _client.PostAsJsonAsync("/api/tasks", payload);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Health_Endpoint_Returns_Ok()
    {
        var response = await _client.GetAsync("/api/health");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
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
