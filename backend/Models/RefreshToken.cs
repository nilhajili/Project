using System;


namespace SmartLifePlanner.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string JwtId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }

        public Guid? UserId { get; set; }
        public User? User { get; set; } 
        public DateTime? RevokedAt { get; set; }
        
        public bool IsActive => RevokedAt == null && ExpiresAt > DateTime.UtcNow;
    }
}