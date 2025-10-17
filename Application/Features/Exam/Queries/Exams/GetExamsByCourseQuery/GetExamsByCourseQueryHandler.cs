using Application.Features.Exam.Dtos;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Domain.Common.Results;
using Infrastructure.Interface;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Exam.Queries.Exams.GetExamsByCourseQuery
{
    public class GetExamsByCourseQueryHandler
       : IRequestHandler<GetExamsByCourseQuery, Result<List<ExamDto>>>
    {
        private readonly IExamRepository _repository;
        private readonly IMapper _mapper;
        private readonly HybridCache _cache;

        public GetExamsByCourseQueryHandler(
            IExamRepository repository,
            IMapper mapper,
            HybridCache cache) 
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<Result<List<ExamDto>>> Handle(GetExamsByCourseQuery request, CancellationToken ct)
        {
            var cacheKey = $"Course_{request.CourseId}_Exams";

            // Cache should store only the DTO list
            var dtos = await _cache.GetOrCreateAsync(
                cacheKey,
                async cancellationToken =>
                {

                    var examsResult = await _repository.GetByCourseIdAsync(request.CourseId, cancellationToken);

                    if (examsResult.IsError || examsResult.Value == null)
                        return Result<List<ExamDto>>.FromError(Error.NotFound("No exams found."));

                    var examDtos = _mapper.Map<List<ExamDto>>(examsResult.Value); 

                    return Result<List<ExamDto>>.FromValue(examDtos);
                    
                },
                options: new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromMinutes(30)
                },
                tags: new[] { "Exams", $"Course_{request.CourseId}" },
                cancellationToken: ct
            );

            // dtos is List<ExamDto>? now
            if (dtos == null)
                return Result<List<ExamDto>>.FromError(Error.NotFound("No exams found for this course."));

            return Result<List<ExamDto>>.FromValue(dtos.Value);
        }

    }

}
