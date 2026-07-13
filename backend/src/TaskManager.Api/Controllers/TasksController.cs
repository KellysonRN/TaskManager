using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Tasks.Commands.CreateTask;
using TaskManager.Application.Tasks.Commands.DeleteTask;
using TaskManager.Application.Tasks.Commands.UpdateTask;
using TaskManager.Application.Tasks.Dtos;
using TaskManager.Application.Tasks.Queries.GetAllTasks;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class TasksController : ControllerBase
{
    private readonly CreateTaskHandler _createTaskHandler;
    private readonly DeleteTaskHandler _deleteTaskHandler;
    private readonly GetAllTasksHandler _getAllTasksHandler;
    private readonly UpdateTaskHandler _updateTaskHandler;

    public TasksController(
        CreateTaskHandler createTaskHandler,
        DeleteTaskHandler deleteTaskHandler,
        UpdateTaskHandler updateTaskHandler,
        GetAllTasksHandler getAllTasksHandler)
    {
        _createTaskHandler = createTaskHandler;
        _deleteTaskHandler = deleteTaskHandler;
        _updateTaskHandler = updateTaskHandler;
        _getAllTasksHandler = getAllTasksHandler;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TaskDto>>> GetAll(CancellationToken cancellationToken)
    {
        var tasks = await _getAllTasksHandler.HandleAsync(new GetAllTasksQuery(), cancellationToken);
        return Ok(tasks);
    }

    [HttpPost]
    public async Task<ActionResult<TaskDto>> Create([FromBody] CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var createdTask = await _createTaskHandler.HandleAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Create), new { id = createdTask.Id }, createdTask);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TaskDto>> Update(Guid id, [FromBody] UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        request.Id = id;
        var updatedTask = await _updateTaskHandler.HandleAsync(request, cancellationToken);
        return Ok(updatedTask);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _deleteTaskHandler.HandleAsync(new DeleteTaskCommand { Id = id }, cancellationToken);
        return NoContent();
    }
}
