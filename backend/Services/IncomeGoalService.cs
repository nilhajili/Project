using Microsoft.EntityFrameworkCore;
using SmartLifePlanner.Data;
using SmartLifePlanner.DTOs;
using SmartLifePlanner.Models;
using SmartLifePlanner.Services.Interfaces;

namespace SmartLifePlanner.Services.Implementations;

public class IncomeGoalService : IIncomeGoalService
{
    private readonly AppDbContext _context;

    public IncomeGoalService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IncomeGoalResponseDto> CreateGoalAsync(Guid userId, IncomeGoalRequestDto dto)
    {
        var goal = new IncomeGoal
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TargetAmount = dto.TargetAmount,
            CurrentAmount = dto.CurrentAmount,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate
        };

        await _context.IncomeGoals.AddAsync(goal);
        await _context.SaveChangesAsync();

        return Calculate(goal);
    }

    public async Task<List<IncomeGoalResponseDto>> GetUserGoalsAsync(Guid userId)
    {
        var goals = await _context.IncomeGoals
            .Where(x => x.UserId == userId)
            .ToListAsync();

        return goals.Select(g => Calculate(g)).ToList();
    }

    public async Task<IncomeGoalResponseDto?> GetGoalByIdAsync(Guid id)
    {
        var goal = await _context.IncomeGoals.FindAsync(id);

        if (goal == null)
            return null;

        return Calculate(goal);
    }

    public async Task<bool> UpdateCurrentAmountAsync(Guid id, decimal amount)
    {
        var goal = await _context.IncomeGoals.FindAsync(id);

        if (goal == null)
            return false;

        goal.CurrentAmount += amount;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteGoalAsync(Guid id)
    {
        var goal = await _context.IncomeGoals.FindAsync(id);

        if (goal == null)
            return false;

        _context.IncomeGoals.Remove(goal);

        await _context.SaveChangesAsync();

        return true;
    }

    private IncomeGoalResponseDto Calculate(IncomeGoal goal)
    {
        var remaining = goal.TargetAmount - goal.CurrentAmount;

        var daysLeft = (goal.EndDate - DateTime.UtcNow).Days;

        if (daysLeft < 1)
            daysLeft = 1;

        var requiredPerDay = remaining / daysLeft;

        return new IncomeGoalResponseDto
        {
            Id = goal.Id,
            TargetAmount = goal.TargetAmount,
            CurrentAmount = goal.CurrentAmount,
            RemainingAmount = remaining,
            DaysLeft = daysLeft,
            RequiredPerDay = requiredPerDay,
            StartDate = goal.StartDate,
            EndDate = goal.EndDate
        };
    }
}