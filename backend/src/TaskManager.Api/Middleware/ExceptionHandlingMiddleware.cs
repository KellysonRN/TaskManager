using System.Net;
using System.Text.Json;

namespace TaskManager.Api.Middleware;

/// <summary>
/// Middleware that catches unhandled exceptions and returns a consistent JSON error response.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionHandlingMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">The logger used to record exception details.</param>
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Executes the middleware logic for the current HTTP request.
    /// </summary>
    /// <param name="context">The HTTP context for the current request.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred while processing request.");
            context.Response.Clear();
            context.Response.ContentType = "application/json";

            if (ex is TaskManager.Application.Common.Cqrs.ValidationException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var payload = JsonSerializer.Serialize(new { error = ex.Message });
                await context.Response.WriteAsync(payload);
                return;
            }

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var fallbackPayload = JsonSerializer.Serialize(new { error = "An unexpected error occurred." });
            await context.Response.WriteAsync(fallbackPayload);
        }
    }
}
