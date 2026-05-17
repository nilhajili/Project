namespace SmartLifePlanner.DTOs;

public class IncomeGoalResponseDto
{
    public Guid Id { get; set; }

    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }

    public decimal RemainingAmount { get; set; }

    public int DaysLeft { get; set; }

    public decimal RequiredPerDay { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}