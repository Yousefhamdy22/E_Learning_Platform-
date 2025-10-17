using Application.Common.AuthDto;
using Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Behaviours.Interfaces
{
    public interface IUserService
    {
        //Guid GetCurrentUserId();
        //Task<bool> UserExistsAsync(Guid userId);

        Task<Result<UserDto>> GetUserByIdAsync(Guid userId);



    }
}
