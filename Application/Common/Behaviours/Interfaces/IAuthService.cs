using Application.Common.AuthDto;
using Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Behaviours.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult> RegisterAsync(RegisterRequest request);
        Task<AuthResult> AuthenticateAsync(string email, string password);
        //Task<AuthResult> RefreshTokenAsync(string expiredToken, string refreshToken);
        Task<bool> RevokeTokensAsync(Guid userId);

    }
}
