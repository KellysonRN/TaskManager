using System.Net;
using System.Net.Http.Json;
using Xunit;
using Shouldly;

namespace TaskManager.IntegrationTests;

[Collection("Integration")]
public sealed class AuthControllerTests
{
    private readonly HttpClient _client;

    public AuthControllerTests(IntegrationTestFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_WithValidCredentials_Returns200WithToken()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "admin@taskmanager.dev",
            password = "Admin@123"
        });

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<TokenResponse>();
        body.ShouldNotBeNull();
        body.Token.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_WithWrongPassword_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "admin@taskmanager.dev",
            password = "WrongPassword"
        });

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithUnknownEmail_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "nobody@unknown.dev",
            password = "Admin@123"
        });

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithEmptyEmail_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "",
            password = "Admin@123"
        });

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    private sealed record TokenResponse(string Token);
}
