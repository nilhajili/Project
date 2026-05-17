using System;

namespace SmartLifePlanner.Models
{
    public class StudyGoal
    {
        public Guid Id { get; set; }         
        public Guid? UserId { get; set; }
        public User? User { get; set; }

        public Guid SubjectId { get; set; }
        public Subject? Subject { get; set; }

        public int TargetHours { get; set; }  
        public int CurrentHours { get; set; } 
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(30);
    }
}