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

namespace Application.Features.Exam.Queries.Answers.GetAnswerOptionsByQuestionIdQuery
{
    public class GetAnswerOptionsByQuestionIdQueryHandler
          : IRequestHandler<GetAnswerOptionsByQuestionIdQuery, Result<List<AnswerOptionDto>>>
    {
        private readonly IAnswerOptionRepository _repository;
        private readonly IMapper _mapper;
        private readonly HybridCache _cache;

        public GetAnswerOptionsByQuestionIdQueryHandler(
            IAnswerOptionRepository repository,
            IMapper mapper,
            HybridCache cache)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<Result<List<AnswerOptionDto>>> Handle(GetAnswerOptionsByQuestionIdQuery request, CancellationToken ct)
        {
            var cacheKey = $"Question_{request.QuestionId}_AnswerOptions";

            var dtos = await _cache.GetOrCreateAsync(
                cacheKey,
                async cancellationToken =>
                {
                    var options = await _repository.GetByQuestionIdAsync(request.QuestionId, cancellationToken);

                    if (options == null || !options.Any())
                        return new List<AnswerOptionDto>();

                    return _mapper.Map<List<AnswerOptionDto>>(options);
                },
                options: new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromMinutes(30)
                },
                tags: new[] { "AnswerOptions", $"Question_{request.QuestionId}" },
                cancellationToken: ct
            );

            return Result<List<AnswerOptionDto>>.FromValue(dtos);
        }
    }

}
