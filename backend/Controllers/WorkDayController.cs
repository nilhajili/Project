using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLifePlanner.DTOs;
using SmartLifePlanner.Services.Interfaces;

namespace SmartLifePlanner.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WorkDayController : ControllerBase
{
    private readonly IWorkDayService _service;

    public WorkDayController(IWorkDayService service)
    {
        _service = service;
    }

    [HttpPost("{userId}")]
    public async Task<IActionResult> AddWorkDay(Guid userId, WorkDayRequestDto dto)
    {
        var result = await _service.AddWorkDayAsync(userId, dto);

        return Ok(result);
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserWorkDays(Guid userId)
    {
        var workDays = await _service.GetUserWorkDaysAsync(userId);

        return Ok(workDays);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWorkDay(Guid id)
    {
        var success = await _service.DeleteWorkDayAsync(id);

        if (!success)
            return NotFound();

        return Ok();
    }
}