using Microsoft.EntityFrameworkCore;
using SmartLifePlanner.Data;
using SmartLifePlanner.DTOs;
using SmartLifePlanner.Services.Interfaces;

namespace SmartLifePlanner.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto?> GetUserProfileAsync(Guid userId)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return null;

        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            
        };
    }

    public async Task<UserDto?> UpdateUserAsync(Guid userId, string fullName, string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return null;

        user.FullName = fullName;
        user.Email = email;

        await _context.SaveChangesAsync();

        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            
        };
    }

    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        var user = await _context.Users
            .Include(u => u.Subjects)
            .ThenInclude(s => s.Tasks) // Subject -> Tasks
            .Include(u => u.Tasks)
            .Include(u => u.StudyGoals)
            .Include(u => u.WorkDays)
            .Include(u => u.IncomeGoals)
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) throw new Exception("User not found");
        foreach(var subject in user.Subjects)
        {
            _context.Tasks.RemoveRange(subject.Tasks);
        }
        
        _context.Subjects.RemoveRange(user.Subjects);
        
        _context.Tasks.RemoveRange(user.Tasks);
        
        _context.StudyGoals.RemoveRange(user.StudyGoals);
        
        _context.WorkDays.RemoveRange(user.WorkDays);
        
        _context.IncomeGoals.RemoveRange(user.IncomeGoals);
        
        _context.RefreshTokens.RemoveRange(user.RefreshTokens);
        
        _context.Users.Remove(user);

        await _context.SaveChangesAsync();

        return true;
    }
    public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
            return false;
        
        bool isCorrect = BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash);

        if (!isCorrect)
            return false;
        
        string newHashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

        user.PasswordHash = newHashedPassword;

        await _context.SaveChangesAsync();

        return true;
    }
}