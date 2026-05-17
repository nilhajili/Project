using ModelsTaskStatus = SmartLifePlanner.Models.TaskStatus;
namespace SmartLifePlanner.DTOs;

public class TaskResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime? Deadline { get; set; }
    public ModelsTaskStatus Status { get; set; } 
    public Guid? SubjectId { get; set; }
}