namespace SmartLifePlanner.DTOs;

public class TaskDto
{
    public Guid UserId { get; set; }

    public Guid SubjectId { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public DateTime DueDate { get; set; }
}