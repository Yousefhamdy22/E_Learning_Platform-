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

namespace Application.Features.Exam.Commands.Questions.RemoveQuestions
{
    public class RemoveQuestionCommandHandler
     : IRequestHandler<RemoveQuestionCommand, Result<Success>>
    {
        private readonly IQuestionRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache;

        public RemoveQuestionCommandHandler(
            IQuestionRepository repository,
            IUnitOfWork unitOfWork,
            IMemoryCache cache)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<Result<Success>> Handle(RemoveQuestionCommand request, CancellationToken ct)
        {
            // 1. Load Question
            var question = await _repository.GetByIdAsync(request.QuestionId, ct);
            if (question == null)
                return Result<Success>.FromError(Error.NotFound("Question.NotFound", "Question not found"));

            // 2. Delete
            await _repository.DeleteAsync(question , ct);
            await _unitOfWork.CommitAsync(ct);

            // 3. Remove from cache
            //_cache.Remove($"Exam_{question.ExamId}_Question_{question.Id}");

            return Result<Success>.FromValue(new Success());
        }
    }
}
