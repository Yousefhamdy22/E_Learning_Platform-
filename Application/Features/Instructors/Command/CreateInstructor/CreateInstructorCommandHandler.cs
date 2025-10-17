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

namespace Application.Features.Instructors.Command.CreateInstructor
{
    public class CreateInstructorCommandHandler : IRequestHandler<CreateInstructorCommand, Result<InstructorDto>>
    {
        private readonly IGenaricRepository<Instructor> _instructorRepository;
        private readonly IUserService _userService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HybridCache _cache;
        private readonly ILogger<CreateInstructorCommandHandler> _logger;

        public CreateInstructorCommandHandler(
            IGenaricRepository<Instructor> instructorRepository,
            IUserService userService,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            HybridCache cache,
            ILogger<CreateInstructorCommandHandler> logger)
        {
            _instructorRepository = instructorRepository;
            _userService = userService;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
            _logger = logger;
        }

        public async Task<Result<InstructorDto>> Handle(CreateInstructorCommand request, CancellationToken ct)
        {
            try
            {
                // Get current user
                //if (_currentUserService.UserId is not Guid userId)
                //    return Result<InstructorDto>.FromError(Error.Unauthorized("User not authenticated"));

                var targetUserid = request.userid;

                // Get user data
                var userResult = await _userService.GetUserByIdAsync(targetUserid);
                if (!userResult.IsSuccess)
                    return Result<InstructorDto>.FromError(Error.NotFound("User.NotFound", "User not found"));

                var user = userResult.Value;

                _logger.LogInformation("Creating instructor for user {UserId}", targetUserid);

                // Check if instructor already exists
                var existingInstructor = await _instructorRepository.GetByUserIdAsnc(targetUserid, ct);
                if (existingInstructor != null)
                    return Result<InstructorDto>.FromError(Error.Conflict("Instructor profile already exists"));

                // Create instructor
                var instructorResult = Instructor.Create(targetUserid, request.title);
                if (!instructorResult.IsSuccess)
                    return Result<InstructorDto>.FromError(Error.Failure());

                var instructor = instructorResult.Value;
                await _instructorRepository.AddAsync(instructor, ct);
                await _unitOfWork.CommitAsync(ct);

                // Cache the result
                var instructorDto = new InstructorDto
                {
                    Id = instructor.Id,
                    UserId = instructor.UserId,
                    // User data
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Title = request.title,

                };

                await _cache.SetAsync($"instructor_{instructor.Id}", instructorDto);
                await _cache.RemoveByTagAsync("instructors", ct);

                _logger.LogInformation("Instructor created with ID: {InstructorId}", instructor.Id);
                return Result<InstructorDto>.FromValue(instructorDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating instructor for user {UserId}", _currentUserService.UserId);
                return Result<InstructorDto>.FromError(Error.Failure("Instructor.CreationFailed", ex.Message));
            }
        }
    }
}
