using SmartLifePlanner.DTOs;

namespace SmartLifePlanner.Services.Interfaces;

public interface IIncomeGoalService
{
    Task<IncomeGoalResponseDto> CreateGoalAsync(Guid userId, IncomeGoalRequestDto dto);

    Task<List<IncomeGoalResponseDto>> GetUserGoalsAsync(Guid userId);

    Task<IncomeGoalResponseDto?> GetGoalByIdAsync(Guid id);

    Task<bool> UpdateCurrentAmountAsync(Guid id, decimal amount);

    Task<bool> DeleteGoalAsync(Guid id);
}