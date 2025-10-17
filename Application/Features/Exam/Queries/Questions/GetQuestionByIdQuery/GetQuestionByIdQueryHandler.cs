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

namespace Application.Features.Exam.Queries.Questions.GetQuestionByIdQuery
{
    public class GetQuestionByIdQueryHandler
     : IRequestHandler<GetQuestionByIdQuery, Result<QuestionDto>>
    {
        private readonly IQuestionRepository _repository;
        private readonly IMapper _mapper;
        private readonly HybridCache _cache;

        public GetQuestionByIdQueryHandler(
            IQuestionRepository repository,
            IMapper mapper,
            HybridCache cache)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<Result<QuestionDto>> Handle(GetQuestionByIdQuery request, CancellationToken ct)
        {
            var cacheKey = $"Question_{request.QuestionId}";

            var dto = await _cache.GetOrCreateAsync(
                cacheKey,
                async cancellationToken =>
                {
                    var question = await _repository.GetByIdAsync(request.QuestionId, cancellationToken);
                    if (question == null) return null;

                    return _mapper.Map<QuestionDto>(question);
                },
                options: new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromMinutes(30)
                },
                tags: new[] { "Questions" },
                cancellationToken: ct
            );

            if (dto == null)
                return Result<QuestionDto>.FromError(
                    Error.NotFound("Question.NotFound", "Question not found"));

            return Result<QuestionDto>.FromValue(dto);
        }
    }
}
