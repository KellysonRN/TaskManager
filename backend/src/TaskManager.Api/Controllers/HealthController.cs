using Microsoft.AspNetCore.Mvc;

namespace TaskManager.Api.Controllers;

/// <summary>
/// Provides health check endpoints for the TaskManager API.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class HealthController : ControllerBase
{
    /// <summary>
    /// Returns the current health status of the API.
    /// </summary>
    /// <returns>A 200 OK response containing the health status.</returns>
    [HttpGet]
    public IActionResult Get() => Ok(new { status = "Healthy" });
}
