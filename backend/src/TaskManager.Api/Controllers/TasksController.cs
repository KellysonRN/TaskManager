using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Tasks.Commands.CreateTask;
using TaskManager.Application.Tasks.Dtos;
using TaskManager.Application.Tasks.Queries.GetAllTasks;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class TasksController : ControllerBase
{
    private readonly CreateTaskHandler _createTaskHandler;
    private readonly GetAllTasksHandler _getAllTasksHandler;

    public TasksController(CreateTaskHandler createTaskHandler, GetAllTasksHandler getAllTasksHandler)
    {
        _createTaskHandler = createTaskHandler;
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
}
