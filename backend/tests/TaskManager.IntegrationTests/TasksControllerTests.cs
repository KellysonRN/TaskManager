using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Shouldly;
using TaskManager.Application.Tasks.Dtos;
using Xunit;

namespace TaskManager.IntegrationTests;

[Collection("Integration")]
public sealed class TasksControllerTests
{
    private readonly HttpClient _client;

    public TasksControllerTests(IntegrationTestFactory factory)
    {
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
    public async Task CreateTask_Returns_Created_Status_And_Created_Task()
    {
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

    private sealed record TokenResponse(string Token);
}
