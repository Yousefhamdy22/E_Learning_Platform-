using Domain.Common;
using Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Identity
{

    public sealed class RefreshToken : AuditableEntity
    {
        public string Token { get;  set; } = string.Empty;       
        public Guid UserId { get;  set; }        
        public DateTime ExpiresOnUtc { get;  set; }
        
        public string SessionId { get;  set; } = string.Empty;   
        public bool IsRevoked { get;  set; }                     

        public ApplicationUser User { get; set; } = null!;

      
        public RefreshToken() { } 

        public RefreshToken(string token, Guid userId, DateTime expiresOnUtc, string sessionId)
        {
            Token = token;
            UserId = userId;
            ExpiresOnUtc = expiresOnUtc;
            SessionId = sessionId;
            IsRevoked = false;
        }

        public static Result<RefreshToken> Create(string token, Guid userId,
            DateTime expiresOnUtc, string sessionId)
        {
            
            if (string.IsNullOrEmpty(token))
                return Result<RefreshToken>.FromError
                    (Error.Validation("Token cannot be empty"));
           
            if (string.IsNullOrEmpty(userId.ToString()))
                return Result<RefreshToken>.FromError
                    (Error.Validation("User ID cannot be empty"));
            if (expiresOnUtc <= DateTime.UtcNow)
                return Result<RefreshToken>.FromError
                    (Error.Validation("Token must expire in the future"));

            return Result<RefreshToken>.FromValue(new RefreshToken(token, userId, expiresOnUtc, sessionId));
        }



        public void Revoke()
        {
            IsRevoked = true;
        }
    }
}
