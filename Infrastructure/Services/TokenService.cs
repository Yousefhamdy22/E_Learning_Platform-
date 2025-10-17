using Domain.Entities.Identity;
using Infrastructure.Helper;
using Infrastructure.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ILogger<TokenService> _logger;
        private readonly byte[] _jwtSecret;

        public TokenService(
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            IRefreshTokenRepository refreshTokenRepository,
            ILogger<TokenService> logger)
        {
            _configuration = configuration;
            _userManager = userManager;
            _refreshTokenRepository = refreshTokenRepository;
            _logger = logger;

            var secretKey = _configuration["JwtSettings:Secret"]
                           ?? throw new ArgumentNullException("JWT Secret Key is required");
            _jwtSecret = Encoding.UTF8.GetBytes(secretKey);
        }

        public async Task<(string token, string sessionId)> GenerateTokenAsync(ApplicationUser user)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var now = DateTime.UtcNow;
                var accessTokenMinutes = Convert.ToDouble(_configuration["JwtSettings:AccessTokenExpiration"] ?? "60");
                var expiration = now.AddMinutes(accessTokenMinutes);
                var roles = await _userManager.GetRolesAsync(user);

                // Generate unique session ID for each login
                var sessionId = Guid.NewGuid().ToString();

                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,
                    new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                // Critical for single device validation
                new Claim("session_id", sessionId)
            };

                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    NotBefore = now,
                    Expires = expiration,
                    Issuer = _configuration["JwtSettings:Issuer"],
                    Audience = _configuration["JwtSettings:Audience"],
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(_jwtSecret),
                        SecurityAlgorithms.HmacSha256)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return (tokenString, sessionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating token for user {UserId}", user.Id);
                throw new AuthenticationExceptions("Failed to generate authentication token", ex, "TOKEN_GENERATION_ERROR");
            }
        }

        public string GenerateRefreshToken()
        {
            try
            {
                var randomNumber = new byte[64]; // Increased size for better security
                using var rng = RandomNumberGenerator.Create();
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating refresh token");
                throw new AuthenticationExceptions("Failed to generate refresh token", ex, "REFRESH_TOKEN_ERROR");
            }
        }

        public async Task SaveRefreshTokenAsync(Guid userId, string refreshToken, string sessionId)
        {
            try
            {
                var expiryDays = _configuration["JwtSettings:RefreshTokenExpirationDays"] ?? "30";
                var expiryTime = DateTime.UtcNow.AddDays(Convert.ToDouble(expiryDays));

                await _refreshTokenRepository.SaveRefreshTokenAsync(userId, refreshToken, expiryTime, sessionId);
                _logger.LogDebug("Refresh token saved for user {UserId} with session {SessionId}", userId, sessionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving refresh token for user {UserId}", userId);
                throw new AuthenticationExceptions("Failed to save refresh token", ex, "SAVE_TOKEN_ERROR");
            }
        }

        public async Task RevokeRefreshTokensAsync(Guid userId, string? exceptSessionId = null)
        {
            try
            {
                await _refreshTokenRepository.RevokeAllUserTokensAsync(userId, exceptSessionId);
                _logger.LogInformation("All refresh tokens revoked for user {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking refresh tokens for user {UserId}", userId);
                throw new AuthenticationExceptions("Failed to revoke existing tokens", ex, "REVOKE_TOKEN_ERROR");
            }
        }

        public async Task<bool> IsSessionValidAsync(Guid userId, string sessionId)
        {
            try
            {
                return await _refreshTokenRepository.IsSessionValidAsync(userId, sessionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating session {SessionId} for user {UserId}", sessionId, userId);
                return false;
            }
        }

        public async Task RevokeSessionAsync(Guid userId, string sessionId)
        {
            try
            {
                await _refreshTokenRepository.RevokeSessionAsync(userId, sessionId);
                _logger.LogInformation("Session {SessionId} revoked for user {UserId}", sessionId, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking session {SessionId} for user {UserId}", sessionId, userId);
                throw new AuthenticationExceptions("Failed to revoke session", ex, "REVOKE_SESSION_ERROR");
            }
        }

    }
}
