using SmartLifePlanner.DTOs;

namespace SmartLifePlanner.Services.Interfaces;

public interface IWorkDayService
{
    Task<WorkDayResponseDto> AddWorkDayAsync(Guid userId, WorkDayRequestDto dto);

    Task<List<WorkDayResponseDto>> GetUserWorkDaysAsync(Guid userId);

    Task<bool> DeleteWorkDayAsync(Guid id);
}