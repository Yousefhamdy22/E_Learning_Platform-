using Application.Features.Course.Dtos;
using Application.Features.Exam.Dtos;
using AutoMapper;
using Domain.Common.Results;
using Infrastructure.Interface;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Exam.Queries.Exams.GetExamByIdQuery
{
    public class GetExamByIdQueryHandler
     : IRequestHandler<GetExamByIdQuery, Result<ExamDto>>
    {
        private readonly IExamRepository _repository;
        private readonly IMapper _mapper;
        private readonly HybridCache _cache;

        public GetExamByIdQueryHandler(IExamRepository repository, IMapper mapper, HybridCache cache)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<Result<ExamDto>> Handle(GetExamByIdQuery request, CancellationToken ct)
        {
            var cacheKey = $"Exam_{request.ExamId}";

           var dto = await _cache.GetOrCreateAsync(
               cacheKey,
               async cancellationToken =>
               {
             
                   var exam = await _repository.GetByIdAsync(request.ExamId , ct);
                   if (exam == null)
                       return null;
             
                   return _mapper.Map<ExamDto>(exam);
               },
               options: new HybridCacheEntryOptions
               {
                   Expiration = TimeSpan.FromMinutes(60)
               },
               tags: new[] { "Exam" },
               cancellationToken: ct
           );

           

            if (dto == null)
                return Result<ExamDto>.FromError(Error.NotFound("Exam.NotFound", "Exam not found"));

            return Result<ExamDto>.FromValue(dto);
        }
    }
}
