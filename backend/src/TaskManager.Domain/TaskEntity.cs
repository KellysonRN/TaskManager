namespace TaskManager.Domain;

public class TaskEntity
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Status { get; set; }
    public Guid OwnerId { get; set; }
}
