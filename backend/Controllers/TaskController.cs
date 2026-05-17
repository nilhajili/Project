using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLifePlanner.DTOs;
using SmartLifePlanner.Services.Interfaces;

namespace SmartLifePlanner.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public TaskController(IStudentService studentService)
        {
            _studentService = studentService;
        }
        
        [HttpPost]
        public async Task<ActionResult<TaskResponseDto>> AddTask([FromBody] TaskRequestDto dto)
        {
            var result = await _studentService.AddTaskAsync(dto);
            return Ok(result);
        }
        
        [HttpPut("{taskId}")]
        public async Task<ActionResult<TaskResponseDto>> UpdateTask(Guid taskId, [FromBody] TaskRequestDto dto)
        {
            var result = await _studentService.UpdateTaskAsync(taskId, dto);
         

            return Ok(result);
        }
        
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<TaskResponseDto>>> GetUserTasks(Guid userId)
        {
            var tasks = await _studentService.GetTasksAsync(userId);
            return Ok(tasks);
        }
        [HttpDelete("{taskid}")]
        public async Task<IActionResult> DeleteTask(Guid taskId)
        {
            var success = await _studentService.DeleteTaskAsync(taskId);
            if (!success) return NotFound(new { Message = "Task not found" });

            return NoContent();
        }
    }
}