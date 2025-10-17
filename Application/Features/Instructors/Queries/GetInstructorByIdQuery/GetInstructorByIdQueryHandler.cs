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

namespace Application.Features.Instructors.Queries.GetInstructorByIdQuery
{
    public class GetInstructorByIdQueryHandler : IRequestHandler<GetInstructorByIdQuery, Result<InstructorDto>>
    {
        private readonly IGenaricRepository<Instructor> _instructorRepository;
        private readonly IUserService _userService;
        private readonly HybridCache _cache;
        private readonly ILogger<GetInstructorByIdQueryHandler> _logger;

        public GetInstructorByIdQueryHandler(
            IGenaricRepository<Instructor> instructorRepository,
            IUserService userService,
            HybridCache cache,
            ILogger<GetInstructorByIdQueryHandler> logger)
        {
            _instructorRepository = instructorRepository;
            _userService = userService;
            _cache = cache;
            _logger = logger;
        }

        public async Task<Result<InstructorDto>> Handle(GetInstructorByIdQuery request, CancellationToken ct)
        {
            try
            {
                var cacheKey = $"instructor_{request.InstructorId}";
                var cacheTags = new[] { "instructors", $"instructor_{request.InstructorId}" };





                var instructorDto = await _cache.GetOrCreateAsync(
                    cacheKey,
                    async cancellationToken =>
                    {
                        _logger.LogInformation("Cache miss - loading instructor {InstructorId} from database", request.InstructorId);

                        var instructor = await _instructorRepository.GetByIdAsync(request.InstructorId, ct);
                        if (instructor == null)
                            return null;

                        var userResult = await _userService.GetUserByIdAsync(instructor.UserId);
                        if (!userResult.IsSuccess)
                            return null;

                        var user = userResult.Value;

                        return new InstructorDto
                        {
                            Id = instructor.Id,
                            UserId = instructor.UserId,
                            Email = user.Email,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            PhoneNumber = user.PhoneNumber
                        };
                    },
                     options: new HybridCacheEntryOptions
                     {
                         Expiration = TimeSpan.FromMinutes(60)
                     },
                         tags: new[] { "instructors" },
                         cancellationToken: ct
                     );

                if (instructorDto == null)
                {
                    _logger.LogWarning("Instructor {InstructorId} not found", request.InstructorId);
                    return Result<InstructorDto>.FromError(Error.NotFound("Instructor not found"));
                }

                _logger.LogInformation("Successfully retrieved instructor {InstructorId}", request.InstructorId);
                return Result<InstructorDto>.FromValue(instructorDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting instructor by ID: {InstructorId}", request.InstructorId);
                return Result<InstructorDto>.FromError(Error.Failure("Instructor.RetrievalFailed", ex.Message));
            }
        }
    }
}