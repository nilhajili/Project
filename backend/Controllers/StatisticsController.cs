using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLifePlanner.DTOs;
using SmartLifePlanner.Services.Interfaces;
using System.Security.Claims;

namespace SmartLifePlanner.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StatisticController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStatistics([FromQuery] string period = "weekly")
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty) return Unauthorized();

            var stats = await _studentService.GetStudentStatisticsAsync(userId, period);
            return Ok(stats);
        }

        [HttpGet("tasks")]
        public async Task<IActionResult> GetTaskStatistics([FromQuery] string period = "weekly")
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty) return Unauthorized();

            var stats = await _studentService.GetStudentStatisticsAsync(userId, period);
            return Ok(stats.TaskStats);
        }

        [HttpGet("goals")]
        public async Task<IActionResult> GetGoalStatistics([FromQuery] string period = "weekly")
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty) return Unauthorized();

            var stats = await _studentService.GetStudentStatisticsAsync(userId, period);
            return Ok(stats.StudyGoalStats);
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out Guid userId) ? userId : Guid.Empty;
        }
    }
}