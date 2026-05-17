using SmartLifePlanner.DTOs;
using System.Text.RegularExpressions;

namespace Smart_Life_Planner.Validators
{
    public static class AuthValidators
    {
        public static void ValidateRegister(RegisterRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.FullName))
                throw new ArgumentException("Full name is required.");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("Email is required.");

            if (!Regex.IsMatch(request.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new ArgumentException("Invalid email format.");

            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
                throw new ArgumentException("Password must be at least 6 characters long.");

            if (string.IsNullOrWhiteSpace(request.Role))
                throw new ArgumentException("Role is required.");
        }
        public static void ValidateLogin(LoginRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("Email is required.");

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Password is required.");
        }
        public static void ValidateRefreshToken(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new ArgumentException("Refresh token is required.");
        }
    }
}