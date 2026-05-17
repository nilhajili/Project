namespace SmartLifePlanner.DTOs;

public class StudyGoalStatisticsDto
{
    public Guid SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int TargetHours { get; set; }
    public int CurrentHours { get; set; }
    public double ProgressPercent { get; set; }
}