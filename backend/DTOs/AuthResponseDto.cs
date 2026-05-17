namespace SmartLifePlanner.DTOs;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public DateTime RefreshTokenExpiresAt { get; set; }
    public string Email { get; set; } = string.Empty;
    public ICollection<string> Roles { get; set; } = new List<string>();
}