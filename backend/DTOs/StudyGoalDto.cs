using System.ComponentModel.DataAnnotations.Schema;
using SmartLifePlanner.Models;

namespace SmartLifePlanner.DTOs;


public class StudyGoal
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid SubjectId { get; set; }
    public int TargetHours { get; set; }
    public int CurrentHours { get; set; }
    [ForeignKey("SubjectId")]
    public Subject? Subject { get; set; }
}