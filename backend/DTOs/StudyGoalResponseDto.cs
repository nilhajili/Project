namespace SmartLifePlanner.DTOs;

public class StudyGoalResponseDto
{
    public Guid Id { get; set; }
    public Guid SubjectId { get; set; }   
    public string SubjectName { get; set; } = string.Empty; 
    public int TargetHours { get; set; }
    public int CurrentHours { get; set; }
    public double Progress { get; set; }
}