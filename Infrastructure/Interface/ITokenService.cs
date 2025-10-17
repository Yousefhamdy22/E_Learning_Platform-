using Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interface
{
    public interface ITokenService
    {
        Task<(string token, string sessionId)> GenerateTokenAsync(ApplicationUser user);
        string GenerateRefreshToken();
        Task SaveRefreshTokenAsync(Guid userId, string refreshToken, string sessionId);
        Task RevokeRefreshTokensAsync(Guid userId, string? exceptSessionId = null);
        Task<bool> IsSessionValidAsync(Guid userId, string sessionId);
        Task RevokeSessionAsync(Guid userId, string sessionId);
    }
}
