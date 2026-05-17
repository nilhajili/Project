using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLifePlanner.DTOs;
using SmartLifePlanner.Services.Interfaces;

namespace SmartLifePlanner.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class IncomeGoalController : ControllerBase
{
    private readonly IIncomeGoalService _service;

    public IncomeGoalController(IIncomeGoalService service)
    {
        _service = service;
    }

    [HttpPost("{userId}")]
    public async Task<IActionResult> CreateGoal(Guid userId, IncomeGoalRequestDto dto)
    {
        var result = await _service.CreateGoalAsync(userId, dto);

        return Ok(result);
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserGoals(Guid userId)
    {
        var goals = await _service.GetUserGoalsAsync(userId);

        return Ok(goals);
    }

    [HttpPut("add-income/{goalId}")]
    public async Task<IActionResult> AddIncome(Guid goalId, decimal amount)
    {
        var success = await _service.UpdateCurrentAmountAsync(goalId, amount);

        if (!success)
            return NotFound();

        return Ok();
    }

    [HttpDelete("{goalId}")]
    public async Task<IActionResult> DeleteGoal(Guid goalId)
    {
        var success = await _service.DeleteGoalAsync(goalId);

        if (!success)
            return NotFound();

        return Ok();
    }
}