using Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.AuthDto
{
    public class AuthResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
        public string SessionId { get; set; } = string.Empty;

        public static AuthResult Success(string token, string refreshToken, ApplicationUser user, string sessionId = "")
        {
            return new AuthResult
            {
                IsSuccess = true,
                AccessToken = token,
                RefreshToken = refreshToken,
                User = user,
                SessionId = sessionId,
                Message = "Success"
            };
        }

        public static AuthResult Fail(string message)
        {
            return new AuthResult
            {
                IsSuccess = false,
                Message = message
            };
        }
    }
    public record LoginRequest(string Email, string Password);
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "Token is required")]
        public string Token { get; set; }
        public string AccessToken { get; set; }

        [Required(ErrorMessage = "Refresh token is required")]
        public string RefreshToken { get; set; }
    }

    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt  { get; set; } 
 
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        [Required]
        public string Role { get; set; } = string.Empty;
    }

    public class RegisterResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }

    #region Helper

 
    public static class AppRoles
    {
        public const string Admin = "Admin";
        public const string Instructor = "Instructor";
        public const string Student = "Student";

        public static bool IsValidRole(string role)
        {
            return role == Admin || role == Instructor || role == Student;
        }

        public static object[] GetAllRole()
        {
            return new object[] { Admin, Instructor, Student };
        }

    }
    #endregion

    public enum UserRole
    {
        Student,
        Instructor,
        Admin
    }
}
