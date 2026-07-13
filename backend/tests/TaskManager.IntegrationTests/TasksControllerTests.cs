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

    [Fact]
    public async Task DeleteTask_Returns_NoContent_When_Task_Exists()
    {
        var createPayload = new
        {
            title = "Task to delete",
            description = "Delete endpoint integration",
            dueDate = DateTime.UtcNow.AddDays(1),
            status = "Pending"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/tasks", createPayload);
        createResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskDto>();
        createdTask.ShouldNotBeNull();

        var deleteResponse = await _client.DeleteAsync($"/api/tasks/{createdTask.Id}");

        deleteResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteTask_Returns_NotFound_When_Task_DoesNotExist()
    {
        var deleteResponse = await _client.DeleteAsync($"/api/tasks/{Guid.NewGuid()}");

        deleteResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateTask_Returns_Ok_And_Updated_Task()
    {
        var createPayload = new
        {
            title = "Task to update",
            description = "Before",
            dueDate = DateTime.UtcNow.AddDays(1),
            status = "Pending"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/tasks", createPayload);
        createResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskDto>();
        createdTask.ShouldNotBeNull();

        var updatePayload = new
        {
            title = "Task updated",
            description = "After",
            dueDate = DateTime.UtcNow.AddDays(3),
            status = "InProgress"
        };

        var updateResponse = await _client.PutAsJsonAsync($"/api/tasks/{createdTask.Id}", updatePayload);

        updateResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var updatedTask = await updateResponse.Content.ReadFromJsonAsync<TaskDto>();
        updatedTask.ShouldNotBeNull();
        updatedTask.Title.ShouldBe("Task updated");
        updatedTask.Description.ShouldBe("After");
        updatedTask.Status.ShouldBe("InProgress");
    }

    [Fact]
    public async Task UpdateTask_Returns_NotFound_When_Task_DoesNotExist()
    {
        var updatePayload = new
        {
            title = "Task updated",
            description = "After",
            dueDate = DateTime.UtcNow.AddDays(3),
            status = "InProgress"
        };

        var updateResponse = await _client.PutAsJsonAsync($"/api/tasks/{Guid.NewGuid()}", updatePayload);

        updateResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    private sealed record TokenResponse(string Token);
}
