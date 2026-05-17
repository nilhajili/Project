using System;
using System.Collections.Generic;

namespace SmartLifePlanner.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        
        
        public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
        public ICollection<Task> Tasks { get; set; } = new List<Task>();
        public ICollection<StudyGoal> StudyGoals { get; set; } = new List<StudyGoal>();
        public ICollection<WorkDay> WorkDays { get; set; } = new List<WorkDay>();
        public ICollection<IncomeGoal> IncomeGoals { get; set; } = new List<IncomeGoal>();
        
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
    
}