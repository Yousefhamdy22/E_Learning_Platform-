using Application.Common.AuthDto;
using Application.Common.Behaviours.Interfaces;
using Domain.Common.Results;
using Domain.Entities.Identity;
using Infrastructure.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AuthService : IAuthService
    {


        #region DI

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        private readonly ILogger<AuthService> _logger;
        #endregion

        #region ctors


        public AuthService(
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService,
            IRefreshTokenRepository refreshTokenRepo,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _refreshTokenRepo = refreshTokenRepo;
            _logger = logger;
        }
        #endregion

        #region Authentication

        public async Task<AuthResult> RegisterAsync(RegisterRequest request)
        {
            try
            {
                // Input validation
                var validationResult = ValidateRegistrationRequest(request);
                if (!validationResult.IsValid)
                {
                    return AuthResult.Fail(validationResult.ErrorMessage);
                }

                // Check if user already exists
                var existingUser = await _userManager.FindByEmailAsync(request.Email.ToLowerInvariant());
                if (existingUser != null)
                {
                    _logger.LogWarning("Registration failed: User already exists with email {Email}", request.Email);
                    return AuthResult.Fail("An account with this email already exists");
                }

                // Create new user
                var user = new ApplicationUser
                {
                    UserName = request.Email.ToLowerInvariant(),
                    Email = request.Email.ToLowerInvariant(),
                    FirstName = request.FirstName?.Trim(),
                    LastName = request.LastName?.Trim(),
                    PhoneNumber = request.PhoneNumber?.Trim(),
                    EmailConfirmed = false,
                    CreatedDate = DateTime.UtcNow,
                    LockoutEnabled = true // Enable lockout for security
                };

                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogWarning("User registration failed for {Email}: {Errors}", request.Email, errors);
                    return AuthResult.Fail($"Registration failed: {errors}");
                }

                // Add role
                await _userManager.AddToRoleAsync(user, request.Role);

                // Auto-login after registration with single device enforcement
                await _tokenService.RevokeRefreshTokensAsync(user.Id);
                var (accessToken, sessionId) = await _tokenService.GenerateTokenAsync(user);
                var refreshToken = _tokenService.GenerateRefreshToken();
                await _tokenService.SaveRefreshTokenAsync(user.Id, refreshToken, sessionId);

                _logger.LogInformation("User registered and logged in successfully: {Email} with session {SessionId}",
                    request.Email, sessionId);

                return AuthResult.Success(accessToken, refreshToken, user, sessionId);
            }
            catch (AuthenticationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during registration for {Email}", request.Email);
                return AuthResult.Fail("An unexpected error occurred during registration. Please try again.");
            }
        }

        public async Task<AuthResult> AuthenticateAsync(string email, string password)
        {
            try
            {
                // Input validation
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    _logger.LogWarning("Authentication failed: Empty email or password");
                    return AuthResult.Fail("Email and password are required");
                }

                var user = await _userManager.FindByEmailAsync(email.ToLowerInvariant());
                if (user == null)
                {
                    _logger.LogWarning("Authentication failed: User not found for email {Email}", email);
                    return AuthResult.Fail("Invalid credentials");
                }

                // Check if account is locked
                if (await _userManager.IsLockedOutAsync(user))
                {
                    _logger.LogWarning("Authentication failed: Account locked for user {UserId}", user.Id);
                    return AuthResult.Fail("Account is temporarily locked. Please try again later.");
                }

                // Validate password
                if (!await _userManager.CheckPasswordAsync(user, password))
                {
                    await _userManager.AccessFailedAsync(user); // Track failed attempts
                    _logger.LogWarning("Authentication failed: Invalid password for user {UserId}", user.Id);
                    return AuthResult.Fail("Invalid credentials");
                }

                // Reset access failed count on successful login
                await _userManager.ResetAccessFailedCountAsync(user);

                // SINGLE DEVICE ENFORCEMENT - Revoke all existing sessions
                await _tokenService.RevokeRefreshTokensAsync(user.Id);
                _logger.LogInformation("All existing sessions revoked for single-device login for user {UserId}", user.Id);

                // Generate new tokens
                var (accessToken, sessionId) = await _tokenService.GenerateTokenAsync(user);
                var refreshToken = _tokenService.GenerateRefreshToken();

                // Save the new session
                await _tokenService.SaveRefreshTokenAsync(user.Id, refreshToken, sessionId);

                _logger.LogInformation("User {UserId} authenticated successfully with session {SessionId}", user.Id, sessionId);

                return AuthResult.Success(accessToken, refreshToken, user, sessionId);
            }
            catch (AuthenticationException)
            {
                throw; 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during authentication for email {Email}", email);
                return AuthResult.Fail("An unexpected error occurred during authentication");
            }
        }


        //public async Task<AuthResult> RefreshTokenAsync(string expiredToken, string refreshToken)
        //{
        //    try
        //    {
        //        var principal = _tokenService.GetPrincipalFromExpiredToken(expiredToken);
        //        if (principal == null)
        //        {
        //            _logger.LogWarning("Refresh token failed: Invalid expired token");
        //            return AuthResult.Fail("Invalid token");
        //        }

        //        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        //        //var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        //        //if (!Guid.TryParse(userId, out var userIuserIddGuid))
        //        //{
        //        //    // Handle error: claim is missing or not a valid Guid
        //        //    throw new Exception("Invalid User ID in claims");
        //        //}

        //        if (string.IsNullOrEmpty(userId))
        //        {
        //            _logger.LogWarning("Refresh token failed: Missing user ID in token");
        //            return AuthResult.Fail("Invalid token");
        //        }

        //        var validUserId = await _tokenService.ValidateRefreshTokenAsync(refreshToken);
        //        if (string.IsNullOrEmpty(validUserId) || validUserId != userId)
        //        {
        //            _logger.LogWarning("Refresh token failed: Invalid refresh token for user {UserId}", userId);
        //            return AuthResult.Fail("Invalid refresh token");
        //        }
        //        var user = await _userManager.FindByIdAsync(userId);
        //        if (user == null)
        //        {
        //            _logger.LogWarning("Refresh token failed: User {UserId} not found", user);
        //            return AuthResult.Fail("User not found");
        //        }

        //        // Generate new tokens
        //        var newToken = await _tokenService.GenerateTokenAsync(user);
        //        var newRefreshToken = _tokenService.GenerateRefreshToken();
        //        var sessionId = Guid.NewGuid().ToString();

        //        // Revoke old refresh token and save new one
        //        await _tokenService.RevokeRefreshTokensAsync(Guid.Parse(userId));
        //        await _tokenService.SaveRefreshTokenAsync(Guid.Parse(userId), newRefreshToken , sessionId);

        //        _logger.LogInformation("Token refreshed successfully for user {UserId}", userId);

        //        return AuthResult.Success(
        //            token: newToken.ToString(),
        //            refreshToken: newRefreshToken,
        //            user: user);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error during token refresh");
        //        return AuthResult.Fail("An error occurred during token refresh");
        //    }
        //}

        public async Task<bool> RevokeTokensAsync(Guid userId)
        {
            try
            {
                await _tokenService.RevokeRefreshTokensAsync(userId);
                _logger.LogInformation("Tokens revoked for user {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking tokens for user {UserId}", userId);
                return false;
            }
        }
        #endregion

        #region Validate Register


        private (bool IsValid, string ErrorMessage) ValidateRegistrationRequest(RegisterRequest request)
        {
            if (request == null)
                return (false, "Registration request is required");

            if (string.IsNullOrWhiteSpace(request.Email))
                return (false, "Email is required");

            if (string.IsNullOrWhiteSpace(request.Password))
                return (false, "Password is required");

            if (request.Password != request.ConfirmPassword)
                return (false, "Password and confirmation password do not match");

            if (string.IsNullOrWhiteSpace(request.FirstName))
                return (false, "First name is required");

            if (string.IsNullOrWhiteSpace(request.LastName))
                return (false, "Last name is required");

            if (!AppRoles.IsValidRole(request.Role))
                return (false, "Invalid role. Valid roles are: " + string.Join(", ", AppRoles.GetAllRole()));

            // Email format validation
            try
            {
                var addr = new System.Net.Mail.MailAddress(request.Email);
                if (addr.Address != request.Email)
                    return (false, "Invalid email format");
            }
            catch
            {
                return (false, "Invalid email format");
            }

            return (true, string.Empty);
        }

       

        #endregion
    }
}
