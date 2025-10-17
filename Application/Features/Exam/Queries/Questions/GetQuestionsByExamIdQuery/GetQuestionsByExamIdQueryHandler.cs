using Application.Features.Exam.Dtos;
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

namespace Application.Features.Exam.Queries.Questions.GetQuestionsByExamIdQuery
{
    public class GetQuestionsByExamIdQueryHandler
     : IRequestHandler<GetQuestionsByExamIdQuery, Result<List<QuestionDto>>>
    {
        private readonly IQuestionRepository _repository;
        private readonly IMapper _mapper;
        private readonly HybridCache _cache;

        public GetQuestionsByExamIdQueryHandler(
            IQuestionRepository repository,
            IMapper mapper,
            HybridCache cache)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<Result<List<QuestionDto>>> Handle(GetQuestionsByExamIdQuery request, CancellationToken ct)
        {
            var cacheKey = $"Exam_{request.ExamId}_Questions";

            var dtos = await _cache.GetOrCreateAsync(
                cacheKey,
                async cancellationToken =>
                {
                    var questions = await _repository.GetQuestionsByExamIdAsync(request.ExamId, cancellationToken);
                    if (questions == null || !questions.Any())
                        return new List<QuestionDto>();

                    return _mapper.Map<List<QuestionDto>>(questions);
                },
                options: new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromMinutes(30)
                },
                tags: new[] { "Questions", $"Exam_{request.ExamId}" },
                cancellationToken: ct
            );

            return Result<List<QuestionDto>>.FromValue(dtos);
        }
    }
}
