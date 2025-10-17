using Application.Features.Course.Dtos;
using AutoMapper;
using Domain.Common.Interface;
using Domain.Common.Results;
using Domain.Entities.Courses;
using Infrastructure.Interface;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;

using System.Threading;
using System.Threading.Tasks;


namespace Application.Features.Courses.Commands.CreateCourse
{
    public class CreateCourseCommandHandler
       : IRequestHandler<CreateCourseCommand, Result<CourseDto>>
    {
        private readonly ICourseRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateCourseCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly HybridCache _cache;

        public CreateCourseCommandHandler(
            ILogger<CreateCourseCommandHandler> logger,
            ICourseRepository repository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            HybridCache cache)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<Result<CourseDto>> Handle(CreateCourseCommand request, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.TypeStatus))
                return Result<CourseDto>.FromError(Error.Failure("Course type is required"));

            var result = Domain.Entities.Courses.Course.Create(
                request.Title, request.Description, request.TypeStatus,
                request.StartDate, request.EndDate, request.Price, request.InstructorId);

            if (result.IsError)
                return Result<CourseDto>.FromError(result.TopError); 

            var course = result.Value;

            // Database operations
            await _repository.AddAsync(course, ct);
            await _unitOfWork.CommitAsync(ct);

           
            _ = Task.Run(async () =>
            {
                try
                {
                    await _cache.RemoveByTagAsync($"Instructor_{request.InstructorId}", CancellationToken.None);
                    await _cache.RemoveByTagAsync("Courses", CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Background cache invalidation failed");
                }
            });

            var dto = _mapper.Map<CourseDto>(course);
            return Result<CourseDto>.FromValue(dto);
        }
    }
}