using Domain.Common.Interface;
using Domain.Common.Results;
using Domain.Entities.Exams;
using Infrastructure.Interface;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Exam.Commands.Answers.CreateAnswer
{
    public sealed class CreateAnswerOptionCommandHandler
     : IRequestHandler<CreateAnswerOptionCommand, Result<Guid>>
    {
        private readonly IAnswerOptionRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly HybridCache _cache;

        public CreateAnswerOptionCommandHandler(
            IAnswerOptionRepository repository,
            IUnitOfWork unitOfWork,
            HybridCache cache)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<Result<Guid>> Handle(CreateAnswerOptionCommand request, CancellationToken ct)
        {
            // Create entity
            var answerOption = new AnswerOption(
                request.QuestionId,
                request.Text,
                request.IsCorrect
            );

            // Save
            await _repository.AddAsync(answerOption, ct);
            await _unitOfWork.CommitAsync(ct);

            // Invalidate cache for this question's options
            var cacheKey = $"Question_{request.QuestionId}_AnswerOptions";
            await _cache.RemoveAsync(cacheKey, ct);

            return Result<Guid>.FromValue(answerOption.Id);
        }
    }
}
