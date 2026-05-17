namespace SmartLifePlanner.Models;

public class IncomeGoal
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public User? User { get; set; }

    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}