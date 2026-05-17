namespace SmartLifePlanner.DTOs;

public class WorkDayRequestDto
{
    public DateTime Date { get; set; }

    public double HoursWorked { get; set; }

    public string WorkType { get; set; } = string.Empty;
}