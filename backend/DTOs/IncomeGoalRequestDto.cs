namespace SmartLifePlanner.DTOs;

public class IncomeGoalRequestDto
{
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}