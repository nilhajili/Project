using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SmartLifePlanner.Models
{
    public class Subject
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid? UserId { get; set; }
        [JsonIgnore]
        public User? User { get; set; } 
        [JsonIgnore]
        public ICollection<Task> Tasks { get; set; } = new List<Task>();
        public ICollection<StudyGoal> StudyGoals { get; set; } = new List<StudyGoal>();
    }
}