using Application.Common.AuthDto;
using Application.Common.Behaviours.Interfaces;
using Infrastructure.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using System.Security.Claims;


namespace Presentation.Controllers
{
    [Route("api/Auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService , ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage);
                    return BadRequest(new { message = "Invalid input", errors });
                }

                var result = await _authService.AuthenticateAsync(request.Email, request.Password);

                if (!result.IsSuccess)
                {
                    return Unauthorized(new { message = result.Message });
                }

                return Ok(new
                {
                    message = "Login successful",
                    accessToken = result.AccessToken,
                    refreshToken = result.RefreshToken,
                    sessionId = result.SessionId,
                    user = new
                    {
                        id = result.User?.Id,
                        email = result.User?.Email,
                        firstName = result.User?.FirstName,
                        lastName = result.User?.LastName
                    }
                });
            }
            catch (AuthenticationExceptions ex)
            {
                _logger.LogWarning(ex, "Authentication exception during login");
                return Unauthorized(new { message = ex.Message, errorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during login");
                return StatusCode(500, new { message = "An internal server error occurred" });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage);
                    return BadRequest(new { message = "Invalid input", errors });
                }

                var result = await _authService.RegisterAsync(request);

                if (!result.IsSuccess)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(new
                {
                    message = "Registration successful",
                    accessToken = result.AccessToken,
                    refreshToken = result.RefreshToken,
                    sessionId = result.SessionId,
                    user = new
                    {
                        id = result.User?.Id,
                        email = result.User?.Email,
                        firstName = result.User?.FirstName,
                        lastName = result.User?.LastName
                    }
                });
            }
            catch (AuthenticationExceptions ex)
            {
                _logger.LogWarning(ex, "Authentication exception during registration");
                return BadRequest(new { message = ex.Message, errorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during registration");
                return StatusCode(500, new { message = "An internal server error occurred" });
            }
        }


        //[HttpPost("refresh")]
        //public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        //{
        //    var result = await _authService.RefreshTokenAsync(request.AccessToken, request.RefreshToken);

        //    if (!result.IsSuccess)
        //        return Unauthorized(result.Message);

        //    return Ok(new
        //    {
        //        accessToken = result.AccessToken,
        //        refreshToken = result.RefreshToken
        //    });
        //}

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out var userId))
                return BadRequest("Invalid user identifier.");

            await _authService.RevokeTokensAsync(userId);
            return Ok("Logged out successfully");
        }
    }
}
