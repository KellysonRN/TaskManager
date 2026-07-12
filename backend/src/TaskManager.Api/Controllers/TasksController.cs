using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Common.Cqrs;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TasksController : ControllerBase
{
    private readonly CreateTaskHandler _createTaskHandler;

    public TasksController(CreateTaskHandler createTaskHandler)
    {
        _createTaskHandler = createTaskHandler;
    }

    [HttpPost]
    public async Task<ActionResult<TaskDto>> Create([FromBody] CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var createdTask = await _createTaskHandler.HandleAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Create), new { id = createdTask.Id }, createdTask);
    }
}
