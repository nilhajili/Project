namespace SmartLifePlanner.DTOs;

public class WorkDayResponseDto
{
    public Guid Id { get; set; }

    public DateTime Date { get; set; }

    public double HoursWorked { get; set; }

    public string WorkType { get; set; } = string.Empty;
}