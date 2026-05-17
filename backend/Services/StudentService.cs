using Microsoft.EntityFrameworkCore;
using SmartLifePlanner.Data;
using SmartLifePlanner.DTOs;
using SmartLifePlanner.Models;
using SmartLifePlanner.Services.Interfaces;
using TaskEntity = SmartLifePlanner.Models.Task;
using TaskStatusEnum = SmartLifePlanner.Models.TaskStatus; 

namespace SmartLifePlanner.Services;

public static class DateTimeExtensions
{
    public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
    {
        int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
        return dt.AddDays(-1 * diff).Date;
    }
}
public class StudentService : IStudentService
{
    private readonly AppDbContext _context;
    
   

    public StudentService(AppDbContext context)
    {
        _context = context;
    }
    public async Task<TaskResponseDto> AddTaskAsync(TaskRequestDto dto)
    {
        var task = new TaskEntity
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            Deadline = dto.Deadline,
            Status = dto.Status,
            UserId = dto.UserId,
            SubjectId = dto.SubjectId
        };

        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();

        return new TaskResponseDto
        {
            Id = task.Id,
            Name = task.Name,
            Description = task.Description,
            Deadline = task.Deadline,
            Status = task.Status,
            SubjectId = task.SubjectId
        };
    }

    public async Task<TaskResponseDto> UpdateTaskAsync(Guid id, TaskRequestDto dto)
    {
        var task = await _context.Tasks.FirstOrDefaultAsync(x => x.Id == id);
        if (task == null) throw new Exception("Task not found");

        task.Name = dto.Name;
        task.Description = dto.Description;
        task.Deadline = dto.Deadline;
        task.Status = dto.Status;
        task.SubjectId = dto.SubjectId;

        await _context.SaveChangesAsync();

        return new TaskResponseDto
        {
            Id = task.Id,
            Name = task.Name,
            Description = task.Description,
            Deadline = task.Deadline,
            Status = task.Status,
            SubjectId = task.SubjectId
        };
    }

    public async Task<List<TaskResponseDto>> GetTasksAsync(Guid userId)
    {
        return await _context.Tasks
            .Where(x => x.UserId == userId)
            .Select<TaskEntity, TaskResponseDto>(x => new TaskResponseDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Deadline = x.Deadline,
                Status = x.Status,
                SubjectId = x.SubjectId
            })
            .ToListAsync();
    }
    
    public async Task<StudyGoalResponseDto> AddGoalAsync(StudyGoalRequestDto dto)
    {
        var goal = new SmartLifePlanner.Models.StudyGoal
        {
            Id = Guid.NewGuid(),
            SubjectId = dto.SubjectId,
            TargetHours = dto.TargetHours,
            CurrentHours = 0,
            UserId = dto.UserId,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(30)
        };

        await _context.StudyGoals.AddAsync(goal);
        await _context.SaveChangesAsync();

        return new StudyGoalResponseDto
        {
            Id = goal.Id,
            SubjectId = goal.SubjectId,
            TargetHours = goal.TargetHours,
            CurrentHours = goal.CurrentHours,
            Progress = 0
        };
    }

    public async Task<StudyGoalResponseDto?> UpdateCurrentHoursAsync(Guid goalId, int hours)
    {
        var goal = await _context.StudyGoals.FirstOrDefaultAsync(x => x.Id == goalId);
        if (goal == null) return null;

        goal.CurrentHours += hours;
        await _context.SaveChangesAsync();

        return new StudyGoalResponseDto
        {
            Id = goal.Id,
            SubjectId = goal.SubjectId,
            TargetHours = goal.TargetHours,
            CurrentHours = goal.CurrentHours,
            Progress = (double)goal.CurrentHours / goal.TargetHours * 100
        };
    }

    public async Task<double> GetProgressAsync(Guid goalId, Guid subjectId)
    {
        var goal = await _context.StudyGoals
            .FirstOrDefaultAsync(x => x.Id == goalId && x.SubjectId == subjectId);

        if (goal == null) throw new Exception("Goal not found for this subject");

        return (double)goal.CurrentHours / goal.TargetHours * 100;
    }
    public async Task<Subject> AddSubjectAsync(Guid userId, string name)
    {
        var subject = new Subject
        {
            Id = Guid.NewGuid(),
            Name = name,
            UserId = userId
        };

        await _context.Subjects.AddAsync(subject);
        await _context.SaveChangesAsync();

        return subject;
    }
    
    public async Task<List<Subject>> GetUserSubjectsAsync(Guid userId)
    {
        return await _context.Subjects
            .Where(s => s.UserId == userId)
            .Include(s => s.Tasks)
            .Include(s => s.StudyGoals)
            .ToListAsync();
    }
    
    public async Task<Subject?> GetSubjectByIdAsync(Guid subjectId)
    {
        return await _context.Subjects
            .Include(s => s.Tasks)
            .Include(s => s.StudyGoals)
            .FirstOrDefaultAsync(s => s.Id == subjectId);
    }
    public async Task<StudentStatisticsDto> GetStudentStatisticsAsync(Guid userId, string period)
    {
        DateTime startDate = period.ToLower() == "weekly"
            ? DateTime.Now.StartOfWeek(DayOfWeek.Monday)
            : new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

        DateTime endDate = period.ToLower() == "weekly"
            ? startDate.AddDays(7)
            : startDate.AddMonths(1);
        var tasks = await _context.Tasks
            .Where(t => t.UserId == userId && t.Deadline.HasValue && t.Deadline.Value >= startDate && t.Deadline.Value <= endDate)
            .ToListAsync();

        var taskStats = new TaskStatisticsDto
        {
            TotalTasks = tasks.Count,
            CompletedTasks = tasks.Count(t => t.Status == TaskStatusEnum.Done),
            PendingTasks = tasks.Count(t => t.Status == TaskStatusEnum.Pending || t.Status == TaskStatusEnum.InProgress),
            OverdueTasks = tasks.Count(t => t.Deadline < DateTime.Now && t.Status != TaskStatusEnum.Done)
        };
        
        var studyGoals = await _context.StudyGoals
            .Include(g => g.Subject) 
            .Where(g => g.UserId == userId)
            .ToListAsync();

        var studyGoalStats = studyGoals.Select(g => new StudyGoalStatisticsDto
        {
            SubjectId = g.SubjectId,
            SubjectName = g.Subject != null ? g.Subject.Name : "",
            TargetHours = g.TargetHours,
            CurrentHours = g.CurrentHours,
            ProgressPercent = g.TargetHours == 0 ? 0 : (double)g.CurrentHours / g.TargetHours * 100
        }).ToList();

        return new StudentStatisticsDto
        {
            TaskStats = taskStats,
            StudyGoalStats = studyGoalStats
        };
        
    }
    public async Task<bool> DeleteTaskAsync(Guid taskId)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task == null) return false;

        _context.Tasks.Remove(task);

        await _context.SaveChangesAsync();

        return true;
    }
    public async Task<bool> DeleteSubjectAsync(Guid subjectId)
    {
        var subject = await _context.Subjects
            .Include(s => s.Tasks)
            .Include(s => s.StudyGoals)
            .FirstOrDefaultAsync(s => s.Id == subjectId);

        if (subject == null) return false;
        
        _context.Tasks.RemoveRange(subject.Tasks);
        _context.StudyGoals.RemoveRange(subject.StudyGoals);
        
        _context.Subjects.Remove(subject);

        await _context.SaveChangesAsync();

        return true;
    }
    public async Task<bool> DeleteGoalAsync(Guid goalId)
    {
        var goal = await _context.StudyGoals.FindAsync(goalId);
        if (goal == null) return false;

        _context.StudyGoals.Remove(goal);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<StudyGoalResponseDto> UpdateGoalAsync(Guid goalId, UpdateGoalDto dto)
    {
        var goal = await _context.StudyGoals.FindAsync(goalId);
        if (goal == null) throw new Exception("Goal not found");

        goal.CurrentHours = dto.CurrentHours;
        goal.TargetHours = dto.TargetHours; 

        await _context.SaveChangesAsync();

        return new StudyGoalResponseDto
        {
            Id = goal.Id,
            SubjectId = goal.SubjectId,
            CurrentHours = goal.CurrentHours,
            TargetHours = goal.TargetHours
        };
    }
}