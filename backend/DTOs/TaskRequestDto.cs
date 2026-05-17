using ModelsTaskStatus = SmartLifePlanner.Models.TaskStatus;
namespace SmartLifePlanner.DTOs;

public class TaskRequestDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime? Deadline { get; set; }
    public ModelsTaskStatus Status { get; set; } = ModelsTaskStatus.Pending;
    public Guid UserId { get; set; }
    public Guid SubjectId { get; set; }
}
