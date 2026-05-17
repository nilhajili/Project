using Microsoft.EntityFrameworkCore;
using SmartLifePlanner.Data;
using SmartLifePlanner.DTOs;
using SmartLifePlanner.Models;
using SmartLifePlanner.Services.Interfaces;

namespace SmartLifePlanner.Services.Implementations;

public class WorkDayService : IWorkDayService
{
    private readonly AppDbContext _context;

    public WorkDayService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<WorkDayResponseDto> AddWorkDayAsync(Guid userId, WorkDayRequestDto dto)
    {
        var workDay = new WorkDay
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Date = dto.Date,
            HoursWorked = dto.HoursWorked,
            WorkType = dto.WorkType
        };

        await _context.WorkDays.AddAsync(workDay);
        await _context.SaveChangesAsync();

        return new WorkDayResponseDto
        {
            Id = workDay.Id,
            Date = workDay.Date,
            HoursWorked = workDay.HoursWorked,
            WorkType = workDay.WorkType
        };
    }

    public async Task<List<WorkDayResponseDto>> GetUserWorkDaysAsync(Guid userId)
    {
        var workDays = await _context.WorkDays
            .Where(x => x.UserId == userId)
            .ToListAsync();

        return workDays.Select(w => new WorkDayResponseDto
        {
            Id = w.Id,
            Date = w.Date,
            HoursWorked = w.HoursWorked,
            WorkType = w.WorkType
        }).ToList();
    }

    public async Task<bool> DeleteWorkDayAsync(Guid id)
    {
        var workDay = await _context.WorkDays.FindAsync(id);

        if (workDay == null)
            return false;

        _context.WorkDays.Remove(workDay);

        await _context.SaveChangesAsync();

        return true;
    }
}