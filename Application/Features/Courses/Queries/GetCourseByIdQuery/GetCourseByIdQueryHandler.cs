using Application.Features.Course.Dtos;
using Application.Features.Instructors.Dto;
using AutoMapper;
using Domain.Common.Results;
using Infrastructure.Interface;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Courses.Queries.GetCourseByIdQuery
{
    public class GetCourseByIdQueryHandler : IRequestHandler<GetCourseByIdQuery, Result<CourseDto>>
    {
        private readonly ICourseRepository _repository;
        private readonly IMapper _mapper;
        private readonly HybridCache _cache;

        public GetCourseByIdQueryHandler(ICourseRepository repository, IMapper mapper, HybridCache cache)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<Result<CourseDto>> Handle(GetCourseByIdQuery request, CancellationToken ct)
        {
            var cacheKey = $"Course_{request.Id}";


            var dto = await _cache.GetOrCreateAsync(
                cacheKey,
                async cancellationToken =>
                {
               
                    var course = await _repository.GetByIdAsync(request.Id, ct);
                    if (course == null)
                        return null;
               
               
                    return _mapper.Map<CourseDto>(course);
                },
                options: new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromMinutes(60)
                },
                tags: new[] { "Course" }, 
                cancellationToken: ct
            );
           
            if (dto == null)
                return Result<CourseDto>.FromError(Error.NotFound("Instructor not found"));

            return Result<CourseDto>.FromValue(dto);


        }
    }
}
