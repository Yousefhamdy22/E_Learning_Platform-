using Application.Features.Exam.Dtos;
using AutoMapper;
using Domain.Common.Results;
using Infrastructure.Interface;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Exam.Queries.Questions.GetAllQuestionsQuery
{
    public class GetAllQuestionsQueryHandler
     : IRequestHandler<GetAllQuestionsQuery, Result<List<QuestionDto>>>
    {
        private readonly IQuestionRepository _repository;
        private readonly IMapper _mapper;
        private readonly HybridCache _cache;

        public GetAllQuestionsQueryHandler(
            IQuestionRepository repository,
            IMapper mapper,
            HybridCache cache)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<Result<List<QuestionDto>>> Handle(GetAllQuestionsQuery request, CancellationToken ct)
        {
            var cacheKey = "All_Questions";

          
            var dtos = await _cache.GetOrCreateAsync(
                cacheKey,
                async cancellationToken =>
                {
                    
                    var questionsResult = await _repository.GetAllAsync(cancellationToken);
                  
                    if (questionsResult == null)
                        return new List<QuestionDto>();

                    return _mapper.Map<List<QuestionDto>>(questionsResult);
                },
                options: new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromMinutes(30)
                },
                tags: new[] { "Questions" },
                cancellationToken: ct
            );

            return Result<List<QuestionDto>>.FromValue(dtos);
        }
    }
}
