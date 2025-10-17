using Domain.Entities.Identity;
using Infrastructure.Data;
using Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RefreshTokenRepository> _logger;

        public RefreshTokenRepository(ApplicationDbContext context, ILogger<RefreshTokenRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SaveRefreshTokenAsync(Guid userId, string token, DateTime expiryTime, string sessionId)
        {
            try
            {
                _logger.LogInformation("Saving refresh token for user {UserId} with session {SessionId}", userId, sessionId);

                // Create new token with validation
                var result = RefreshToken.Create(token, userId, expiryTime, sessionId);
                if (!result.IsSuccess)
                {
                    _logger.LogError("Failed to create refresh token for user {UserId}: {Errors}", userId, string.Join(", ", result.Errors));
                    throw new InvalidOperationException($"Failed to create refresh token: {string.Join(", ", result.Errors)}");
                }

                var refreshToken = result.Value;

                // Save to database
                await _context.RefreshTokens.AddAsync(refreshToken);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Refresh token saved successfully for user {UserId} with session {SessionId}", userId, sessionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving refresh token for user {UserId}", userId);
                throw;
            }
        }

        public async Task<string?> ValidateRefreshTokenAsync(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    _logger.LogWarning("Refresh token validation failed - empty token provided");
                    return null;
                }

                _logger.LogDebug("Validating refresh token");

                var refreshToken = await _context.RefreshTokens
                    .Include(rt => rt.User)
                    .FirstOrDefaultAsync(rt =>
                        rt.Token == token &&
                        !rt.IsRevoked &&
                        rt.ExpiresOnUtc >= DateTime.UtcNow);

                if (refreshToken == null)
                {
                    _logger.LogWarning("Refresh token validation failed - token not found or invalid");
                    return null;
                }

                // Additional validation: Check if user still exists and is active
                if (refreshToken.User == null)
                {
                    _logger.LogWarning("Refresh token validation failed - user not found for token user ID {UserId}", refreshToken.UserId);
                    await RevokeRefreshTokensAsync(refreshToken.UserId);
                    return null;
                }

                _logger.LogDebug("Refresh token validated successfully for user {UserId}", refreshToken.UserId);
                return refreshToken.UserId.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating refresh token");
                return null;
            }
        }

        public async Task RevokeRefreshTokensAsync(Guid userId)
        {
            try
            {
                _logger.LogInformation("Revoking all refresh tokens for user {UserId}", userId);

                var activeTokens = await _context.RefreshTokens
                    .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                    .ToListAsync();

                _logger.LogInformation("Found {Count} tokens to revoke for user {UserId}", activeTokens.Count, userId);

                foreach (var token in activeTokens)
                {
                    // Use domain method if available, otherwise set property directly
                    if (HasRevokeMethod(token))
                    {
                        token.Revoke(); 
                    }
                    else
                    {
                        token.IsRevoked = true;
                        //token.RevokedAtUtc = DateTime.UtcNow;
                    }
                    _logger.LogDebug("Revoked token {TokenId} for user {UserId}", token.Id, userId);
                }

                if (activeTokens.Any())
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Successfully revoked {Count} tokens for user {UserId}", activeTokens.Count, userId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking refresh tokens for user {UserId}", userId);
                throw;
            }
        }

        public async Task RevokeAllUserTokensAsync(Guid userId, string? exceptSessionId = null)
        {
            try
            {
                _logger.LogInformation("Revoking refresh tokens for user {UserId}, except session {ExceptSessionId}",
                    userId, exceptSessionId ?? "none");

                var query = _context.RefreshTokens
                    .Where(rt => rt.UserId == userId && !rt.IsRevoked);

                if (!string.IsNullOrEmpty(exceptSessionId))
                {
                    query = query.Where(rt => rt.SessionId != exceptSessionId);
                }

                var tokensToRevoke = await query.ToListAsync();

                _logger.LogInformation("Found {Count} tokens to revoke for user {UserId}", tokensToRevoke.Count, userId);

                foreach (var token in tokensToRevoke)
                {
                    if (HasRevokeMethod(token))
                    {
                        token.Revoke();
                    }
                    else
                    {
                        token.IsRevoked = true;
                        //token.RevokedAtUtc = DateTime.UtcNow;
                    }
                    _logger.LogDebug("Revoked token {TokenId} with session {SessionId} for user {UserId}",
                        token.Id, token.SessionId, userId);
                }

                if (tokensToRevoke.Any())
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Successfully revoked {Count} tokens for user {UserId}",
                        tokensToRevoke.Count, userId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking tokens for user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> IsSessionValidAsync(Guid userId, string sessionId)
        {
            try
            {
                if (userId == Guid.Empty || string.IsNullOrWhiteSpace(sessionId))
                {
                    _logger.LogDebug("Session validation failed - invalid input parameters");
                    return false;
                }

                var activeToken = await _context.RefreshTokens
                    .Where(rt =>
                        rt.UserId == userId &&
                        rt.SessionId == sessionId &&
                        !rt.IsRevoked &&
                        rt.ExpiresOnUtc >= DateTime.UtcNow)
                    .FirstOrDefaultAsync();

                var isValid = activeToken != null;

                _logger.LogDebug("Session validation for user {UserId}, session {SessionId}: {IsValid}",
                    userId, sessionId, isValid);

                return isValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating session for user {UserId}, session {SessionId}", userId, sessionId);
                return false;
            }
        }

        public async Task RevokeSessionAsync(Guid userId, string sessionId)
        {
            try
            {
                if (userId == Guid.Empty || string.IsNullOrWhiteSpace(sessionId))
                {
                    _logger.LogWarning("Cannot revoke session - invalid parameters");
                    return;
                }

                _logger.LogInformation("Revoking session {SessionId} for user {UserId}", sessionId, userId);

                var tokensToRevoke = await _context.RefreshTokens
                    .Where(rt =>
                        rt.UserId == userId &&
                        rt.SessionId == sessionId &&
                        !rt.IsRevoked)
                    .ToListAsync();

                _logger.LogInformation("Found {Count} tokens to revoke for session {SessionId}", tokensToRevoke.Count, sessionId);

                foreach (var token in tokensToRevoke)
                {
                    if (HasRevokeMethod(token))
                    {
                        token.Revoke();
                    }
                    else
                    {
                        token.IsRevoked = true;
                        //token.RevokedAtUtc = DateTime.UtcNow;
                    }
                    _logger.LogDebug("Revoked token {TokenId} for session {SessionId}", token.Id, sessionId);
                }

                if (tokensToRevoke.Any())
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Successfully revoked session {SessionId} for user {UserId}", sessionId, userId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking session {SessionId} for user {UserId}", sessionId, userId);
                throw;
            }
        }

        public async Task<string?> GetCurrentSessionIdAsync(Guid userId)
        {
            try
            {
                _logger.LogDebug("Getting current session ID for user {UserId}", userId);

                var currentToken = await _context.RefreshTokens
                    .Where(rt =>
                        rt.UserId == userId &&
                        !rt.IsRevoked &&
                        rt.ExpiresOnUtc >= DateTime.UtcNow)
                    .OrderByDescending(rt => rt.CreatedAtUtc)
                    .FirstOrDefaultAsync();

                var sessionId = currentToken?.SessionId;

                _logger.LogDebug("Current session ID for user {UserId}: {SessionId}", userId, sessionId ?? "none");
                return sessionId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current session ID for user {UserId}", userId);
                return null;
            }
        }

        public async Task<string?> GetActiveTokenIdAsync(Guid userId)
        {
            try
            {
                _logger.LogDebug("Getting active token ID for user {UserId}", userId);

                var activeToken = await _context.RefreshTokens
                    .Where(rt =>
                        rt.UserId == userId &&
                        !rt.IsRevoked &&
                        rt.ExpiresOnUtc >= DateTime.UtcNow)
                    .OrderByDescending(rt => rt.CreatedAtUtc)
                    .FirstOrDefaultAsync();

                var tokenId = activeToken?.Id.ToString();

                _logger.LogDebug("Active token ID for user {UserId}: {TokenId}", userId, tokenId ?? "none");
                return tokenId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active token ID for user {UserId}", userId);
                return null;
            }
        }

        public async Task<RefreshToken?> GetActiveRefreshTokenAsync(Guid userId, string sessionId)
        {
            try
            {
                if (userId == Guid.Empty || string.IsNullOrWhiteSpace(sessionId))
                    return null;

                _logger.LogDebug("Getting active refresh token for user {UserId}, session {SessionId}", userId, sessionId);

                var activeToken = await _context.RefreshTokens
                    .Include(rt => rt.User)
                    .Where(rt =>
                        rt.UserId == userId &&
                        rt.SessionId == sessionId &&
                        !rt.IsRevoked &&
                        rt.ExpiresOnUtc >= DateTime.UtcNow)
                    .FirstOrDefaultAsync();

                _logger.LogDebug("Active refresh token found for user {UserId}, session {SessionId}: {Found}",
                    userId, sessionId, activeToken != null);

                return activeToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active refresh token for user {UserId}, session {SessionId}",
                    userId, sessionId);
                return null;
            }
        }

        public async Task CleanupExpiredTokensAsync()
        {
            try
            {
                _logger.LogInformation("Starting cleanup of expired refresh tokens");

                var expiredTokens = await _context.RefreshTokens
                    .Where(rt => rt.ExpiresOnUtc < DateTime.UtcNow)
                    .ToListAsync();

                if (expiredTokens.Any())
                {
                    _logger.LogInformation("Found {Count} expired tokens to cleanup", expiredTokens.Count);

                    _context.RefreshTokens.RemoveRange(expiredTokens);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Successfully cleaned up {Count} expired tokens", expiredTokens.Count);
                }
                else
                {
                    _logger.LogDebug("No expired tokens found during cleanup");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during expired token cleanup");
                throw;
            }
        }

        // Helper method to check if the RefreshToken entity has a Revoke() method
        private static bool HasRevokeMethod(RefreshToken token)
        {
            return token.GetType().GetMethod("Revoke") != null;
        }
    }
}
