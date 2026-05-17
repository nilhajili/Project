using SmartLifePlanner.DTOs;

namespace SmartLifePlanner.Services.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetUserProfileAsync(Guid userId);
    Task<UserDto?> UpdateUserAsync(Guid userId, string fullName, string email);
    Task<bool> DeleteUserAsync(Guid userId);
    Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
}
