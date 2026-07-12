namespace TaskManager.Application.Common.Cqrs;

public class CreateTaskCommand : IRequest<TaskDto>
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Status { get; set; }
    public Guid? AuthenticatedUserId { get; set; }
}
