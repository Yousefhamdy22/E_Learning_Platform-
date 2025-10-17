using Domain.Common.Interface;
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

namespace Application.Features.Exam.Commands.Exams.RemoveExam
{
    public class DeleteExamCommandHandler : IRequestHandler<DeleteExamCommand, Result<Success>>
    {
        private readonly IExamRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly HybridCache _cache;

        public DeleteExamCommandHandler(IExamRepository repository, IUnitOfWork unitOfWork, HybridCache cache)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<Result<Success>> Handle(DeleteExamCommand request, CancellationToken ct)
        {
            var exam = await _repository.GetByIdAsync(request.Id, ct);
            if (exam == null) return Result<Success>.FromError(Error.NotFound("Exam not found"));

            var result = exam.RemoveExam();
            if (result.IsError) return Result<Success>.FromError(Error.Failure("Error With Delete Exam "));

           await _repository.DeleteAsync(exam , ct);
            await _unitOfWork.CommitAsync(ct);

            await _cache.RemoveAsync($"Exam_{exam.Id}");

            return Result<Success>.FromValue(new Success());
        }
    }
}
