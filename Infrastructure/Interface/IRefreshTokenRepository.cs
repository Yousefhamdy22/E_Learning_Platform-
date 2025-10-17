using Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interface
{
    public interface IRefreshTokenRepository
    {
        Task SaveRefreshTokenAsync(Guid userId, string token, DateTime expiryTime, string sessionId);
        Task<string?> ValidateRefreshTokenAsync(string token);
        Task RevokeRefreshTokensAsync(Guid userId);
        Task RevokeAllUserTokensAsync(Guid userId, string? exceptSessionId = null);
        Task<bool> IsSessionValidAsync(Guid userId, string sessionId);
        Task RevokeSessionAsync(Guid userId, string sessionId);
        Task<string?> GetActiveTokenIdAsync(Guid userId);
        Task<string?> GetCurrentSessionIdAsync(Guid userId);
        Task<RefreshToken?> GetActiveRefreshTokenAsync(Guid userId, string sessionId);
        Task CleanupExpiredTokensAsync();


    }
}
