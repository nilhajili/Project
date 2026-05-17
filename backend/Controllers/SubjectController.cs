using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLifePlanner.Models;
using SmartLifePlanner.Services.Interfaces;
using System.Security.Claims;
using SmartLifePlanner.DTOs;
using SmartLifePlanner.Services;
namespace SmartLifePlanner.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public SubjectController(IStudentService studentService)
        {
            _studentService = studentService;
        }
        
        [HttpPost]
        public async Task<ActionResult<Subject>> AddSubject([FromBody] CreateSubjectDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Subject name is required");
            
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdStr == null) return Unauthorized();

            var userId = Guid.Parse(userIdStr);

            var subject = await _studentService.AddSubjectAsync(userId, dto.Name);
            return Ok(subject);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<Subject>>> GetUserSubjects(Guid userId)
        {
            var subjects = await _studentService.GetUserSubjectsAsync(userId);
            return Ok(subjects);
        }

        [HttpGet("{subjectId}")]
        public async Task<ActionResult<Subject>> GetSubjectById(Guid subjectId)
        {
            var subject = await _studentService.GetSubjectByIdAsync(subjectId);
            if (subject == null) return NotFound();
            return Ok(subject);
        }
        [HttpDelete("{subjectid}")]
        public async Task<IActionResult> DeleteSubject(Guid subjectId)
        {
            var success = await _studentService.DeleteSubjectAsync(subjectId);
            if (!success) return NotFound(new { Message = "Subject  not found" });

            return NoContent();
        }
        
    }
}