using System.Text.Json.Serialization;
using SmartLifePlanner.Models;

namespace SmartLifePlanner.Models
{
public class Task
{
    public Guid Id { get; set; }  
    public string Name { get; set; } = null!;
    
    public string Description { get; set; } = null!;
    public Guid? SubjectId { get; set; }
    

    [JsonIgnore]   
    public Subject? Subject { get; set; } 

    public Guid? UserId { get; set; }

    [JsonIgnore]    
    public User? User { get; set; } 

    public DateTime? Deadline { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.Pending;
}
public enum TaskStatus
{
    Pending,
    InProgress,
    Done
}

}