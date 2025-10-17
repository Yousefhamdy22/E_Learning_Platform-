using Application.Features.Course.Dtos;
using AutoMapper;
using Domain.Common.Interface;
using Domain.Common.Results;
using Infrastructure.Interface;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Courses.Commands.UpdateCourse
{
    public class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, Result<CourseDto>>
    {
        private readonly ICourseRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public UpdateCourseCommandHandler(ICourseRepository repository, IUnitOfWork unitOfWork,
            IMapper mapper, IMemoryCache cache)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<Result<CourseDto>> Handle(UpdateCourseCommand request, CancellationToken ct)
        {
            var course = await _repository.GetByIdAsync(request.Id, ct);
            if (course == null) return Result<CourseDto>.FromError(Error.NotFound("Course not found"));

            var result = course.Update(request.Title, request.Description, request.StartDate, request.EndDate, request.Price);
            if (result.IsError) return Result<CourseDto>.FromError(Error.Failure());

            await _unitOfWork.CommitAsync(ct);

            var dto = _mapper.Map<CourseDto>(course);
            _cache.Set($"Course_{course.Id}", dto, TimeSpan.FromMinutes(60));

            return Result<CourseDto>.FromValue(dto);
        }
    }
}
