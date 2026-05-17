using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLifePlanner.DTOs;
using SmartLifePlanner.Services.Interfaces;


namespace SmartLifePlanner.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class StudyGoalController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudyGoalController(IStudentService studentService)
        {
            _studentService = studentService;
        }
        
        [HttpPost]
        public async Task<ActionResult<StudyGoalResponseDto>> AddGoal([FromBody] StudyGoalRequestDto dto)
        {
            var result = await _studentService.AddGoalAsync(dto);
            return Ok(result);
        }
        
        [HttpPost("{goalId}/add-hours")]
        public async Task<IActionResult> AddHours(Guid goalId, int hours)
        {
            var updatedGoal = await _studentService.UpdateCurrentHoursAsync(goalId, hours);
            if (updatedGoal == null) 
                return NotFound("Study goal not found");

            return Ok(updatedGoal);
        }
        [HttpGet("{goalId}/progress")]
        public async Task<ActionResult<double>> GetProgress(Guid goalId, [FromQuery] Guid subjectId)
        {
            try
            {
                var progress = await _studentService.GetProgressAsync(goalId, subjectId);
                return Ok(progress);
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
        [HttpGet("{subjectId}/goals")]
        public async Task<ActionResult<List<StudyGoalResponseDto>>> GetGoalsBySubject(Guid subjectId)
        {
            var subject = await _studentService.GetSubjectByIdAsync(subjectId);
            if (subject == null) return NotFound(new { Message = "Subject not found" });
            
            var goalsDto = subject.StudyGoals.Select(g => new StudyGoalResponseDto
            {
                Id = g.Id,
                SubjectId = g.SubjectId,
                CurrentHours = g.CurrentHours,
                TargetHours = g.TargetHours
            }).ToList();

            return Ok(goalsDto);
        }

        [HttpDelete("{subjectId}/goals/{goalId}")]
        public async Task<IActionResult> DeleteGoal(Guid subjectId, Guid goalId)
        {
            var subject = await _studentService.GetSubjectByIdAsync(subjectId);
            if (subject == null) return NotFound(new { Message = "Subject not found" });

            var goal = subject.StudyGoals.FirstOrDefault(g => g.Id == goalId);
            if (goal == null) return NotFound(new { Message = "Goal not found" });

            var success = await _studentService.DeleteGoalAsync(goalId);
            if (!success) return BadRequest(new { Message = "Failed to delete goal" });

            return NoContent();
        }

        [HttpPut("{subjectId}/goals/{goalId}")]
        public async Task<IActionResult> UpdateGoal(Guid subjectId, Guid goalId, [FromBody] UpdateGoalDto dto)
        {
            var subject = await _studentService.GetSubjectByIdAsync(subjectId);
            if (subject == null) return NotFound(new { Message = "Subject not found" });

            var goal = subject.StudyGoals.FirstOrDefault(g => g.Id == goalId);
            if (goal == null) return NotFound(new { Message = "Goal not found" });

            var updatedGoal = await _studentService.UpdateGoalAsync(goalId, dto);
            return Ok(updatedGoal);
        }
    }
}