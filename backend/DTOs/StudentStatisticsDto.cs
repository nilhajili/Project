namespace SmartLifePlanner.DTOs;

public class StudentStatisticsDto
{
    public TaskStatisticsDto TaskStats { get; set; } = new TaskStatisticsDto();
    public List<StudyGoalStatisticsDto> StudyGoalStats { get; set; } = new List<StudyGoalStatisticsDto>();
}