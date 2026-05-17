namespace SmartLifePlanner.DTOs;

public class StudyGoalRequestDto
{
    public Guid UserId { get; set; }
    public Guid SubjectId { get; set; }  
    public int TargetHours { get; set; }
}