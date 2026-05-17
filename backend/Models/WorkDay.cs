namespace SmartLifePlanner.Models;

public class WorkDay
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public User? User { get; set; } 
    public DateTime Date { get; set; }
    public double HoursWorked { get; set; } 
    public string WorkType { get; set; } = string.Empty;
}