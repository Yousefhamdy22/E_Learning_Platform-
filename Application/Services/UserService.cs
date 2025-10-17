using Application.Common.AuthDto;
using Application.Common.Behaviours.Interfaces;
using Domain.Common.Results;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

           public UserService(
          ApplicationDbContext context )
      
           {
               _context = context;
              
           }
       
        public async Task<Result<UserDto>> GetUserByIdAsync(Guid userId)
        {
            var user = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    PhoneNumber = u.PhoneNumber,
                    UserName = u.UserName
                })
                .FirstOrDefaultAsync();

            return user != null
                ? Result<UserDto>.FromValue(user)
                : Result<UserDto>.FromError(Error.NotFound("User.NotFound", "User not found"));
        }
    }
}
