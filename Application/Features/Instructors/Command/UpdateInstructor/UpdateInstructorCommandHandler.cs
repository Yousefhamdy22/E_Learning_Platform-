using Application.Common.Behaviours.Interfaces;
using Application.Features.Instructors.Dto;
using AutoMapper;
using Domain.Common.Interface;
using Domain.Common.Results;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Instructors.Command.UpdateInstructor
{
    public class UpdateInstructorCommandHandler : IRequestHandler<UpdateInstructorCommand, Result<InstructorDto>>
    {
        private readonly IGenaricRepository<Instructor> _instructorRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly HybridCache _cache;
        private readonly ILogger<UpdateInstructorCommandHandler> _logger;

        public UpdateInstructorCommandHandler(
            IGenaricRepository<Instructor> instructorRepository,
            IUserService userService,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            HybridCache cache,
            ILogger<UpdateInstructorCommandHandler> logger)
        {
            _instructorRepository = instructorRepository;
            _userService = userService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _cache = cache;
            _logger = logger;
        }

        public async Task<Result<InstructorDto>> Handle(UpdateInstructorCommand request, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Updating instructor {InstructorId}", request.InstructorId);

                // Get instructor
                var instructor = await _instructorRepository.GetByIdAsync(request.InstructorId, ct);
                if (instructor == null)
                {
                    _logger.LogWarning("Instructor {InstructorId} not found", request.InstructorId);
                    return Result<InstructorDto>.FromError(Error.NotFound("Instructor not found"));
                }

                // Get user data
                var userResult = await _userService.GetUserByIdAsync(instructor.UserId);
                if (!userResult.IsSuccess)
                {
                  
                    return Result<InstructorDto>.FromError(Error.Failure("User not found for instructor "));
                }

                var user = userResult.Value;

                // Update instructor profile
                var updateResult = instructor.UpdateProfile(request.FirstName, request.LastName, request.Email);
                if (!updateResult.IsSuccess)
                {
                 
                    return Result<InstructorDto>.FromError(Error.Failure("Instructor update validation failed"));
                }

                await _instructorRepository.UpdateAsync(instructor, ct);
                await _unitOfWork.CommitAsync(ct);

                // Invalidate cache
                await _cache.RemoveAsync($"instructor_{request.InstructorId}", ct);
                await _cache.RemoveByTagAsync("instructors", ct);

                _logger.LogInformation("Instructor {InstructorId} updated successfully", request.InstructorId);

                // Return combined response
                return Result<InstructorDto>.FromValue(new InstructorDto
                {
                    Id = instructor.Id,
                    UserId = instructor.UserId,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating instructor {InstructorId}", request.InstructorId);
                return Result<InstructorDto>.FromError(Error.Failure("Instructor.UpdateFailed", ex.Message));
            }
        }
    }

}
