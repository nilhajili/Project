using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartLifePlanner.Data;
using SmartLifePlanner.DTOs;
using SmartLifePlanner.Models;
using SmartLifePlanner.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartLifePlanner.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private const string RefreshTokenType = "refresh";

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    #region Public Methods

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequest)
    {
        if (await _context.Users.AnyAsync(u => u.Email == registerRequest.Email))
            throw new InvalidOperationException("User with this email already exists.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = registerRequest.FullName,
            Email = registerRequest.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password),
            
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return await GenerateTokensAsync(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginRequest.Email)
                   ?? throw new UnauthorizedAccessException("Invalid email or password.");

        if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return await GenerateTokensAsync(user);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var (principal, jti) = ValidateRefreshToken(refreshToken);

        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.JwtId == jti)
            ?? throw new UnauthorizedAccessException("Invalid refresh token.");

        if (!storedToken.IsActive)
            throw new UnauthorizedAccessException("Refresh token expired or revoked.");

        var user = await _context.Users.FindAsync(storedToken.UserId)
                   ?? throw new UnauthorizedAccessException("User not found.");

        storedToken.RevokedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return await GenerateTokensAsync(user);
    }

    public async System.Threading.Tasks.Task RevokeRefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new ArgumentException("Refresh token is required.");

        try
        {
            var (_, jti) = ValidateRefreshToken(refreshToken);

            var storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.JwtId == jti);

            if (storedToken == null)
                return;

            if (!storedToken.IsActive)
                return;

            storedToken.RevokedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
        catch
        {
            
        }
    }

    #endregion

    #region Helpers

    private async Task<AuthResponseDto> GenerateTokensAsync(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var accessExpirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"]!);
        var refreshExpirationDays = int.Parse(jwtSettings["RefreshTokenExpirationDays"]!);

        var accessToken = GenerateAccessToken(user, accessExpirationMinutes);
        var (refreshEntity, refreshJwt) = await CreateRefreshTokenAsync(user.Id, refreshExpirationDays);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(accessExpirationMinutes),
            RefreshToken = refreshJwt,
            RefreshTokenExpiresAt = refreshEntity.ExpiresAt,
            UserId = user.Id,
            Email = user.Email,
            
        };
    }

    private string GenerateAccessToken(User user, int expirationMinutes)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new Claim[]
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<(RefreshToken entity, string jwt)> CreateRefreshTokenAsync(Guid userId, int expirationDays)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var jti = Guid.NewGuid().ToString();
        var expiresAt = DateTime.UtcNow.AddDays(expirationDays);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["RefreshTokenSecretKey"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new Claim[]
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(JwtRegisteredClaimNames.Jti, jti),
            new("token_type", RefreshTokenType)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds
        );

        var jwtString = new JwtSecurityTokenHandler().WriteToken(token);

        var entity = new RefreshToken
        {
            JwtId = jti,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt
        };

        _context.RefreshTokens.Add(entity);
        await _context.SaveChangesAsync();

        return (entity, jwtString);
    }

    private (ClaimsPrincipal principal, string jti) ValidateRefreshToken(string refreshToken)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["RefreshTokenSecretKey"]!));

        var handler = new JwtSecurityTokenHandler();
        var principal = handler.ValidateToken(refreshToken, new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        }, out var validatedToken);

        var jwt = validatedToken as JwtSecurityToken ?? throw new UnauthorizedAccessException("Invalid token");
        if (jwt.Claims.FirstOrDefault(c => c.Type == "token_type")?.Value != RefreshTokenType)
            throw new UnauthorizedAccessException("Invalid token type");

        var jti = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value
                  ?? throw new UnauthorizedAccessException("Invalid token");

        return (principal, jti);
    }

    #endregion
}