using Domain.Common.Interface;
using Domain.Common.Results;
using Domain.Entities.Exams;
using Infrastructure.Interface;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Exam.Commands.Exams.StartExam
{
    public class StartExamCommandHandler : IRequestHandler<StartExamCommand, Result<Guid>>
    {
        private readonly IExamRepository _examRepository;
        private readonly IExamResultRepository _examResultRepository;
        private readonly IUnitOfWork _unitOfWork;

        public StartExamCommandHandler(
            IExamRepository examRepository,
            IExamResultRepository examResultRepository,
            IUnitOfWork unitOfWork)
        {
            _examRepository = examRepository;
            _examResultRepository = examResultRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(StartExamCommand request, CancellationToken ct)
        {
            // Check if exam exists and get total questions
            var exam = await _examRepository.GetExamWithQuestionsAsync(request.ExamId, ct);
            if (exam == null)
                return Result<Guid>.FromError(Error.NotFound("Exam.NotFound", "Exam not found."));

            // Check if student already started this exam
            var existingResult = await _examResultRepository
                .GetByStudentAndExamAsync(request.StudentId, request.ExamId, ct);


            if (existingResult != null && existingResult.Status != "Submitted")
                return Result<Guid>.FromError(Error.Conflict("ExamResult.AlreadyStarted",
                    "Exam already in progress."));

            // Create new exam result
            var examResult = ExamResult.StartExam(
                request.StudentId,
                request.ExamId
                );

            if (examResult.IsError)
                return Result<Guid>.FromError(examResult.TopError);

            await _examResultRepository.AddAsync(examResult.Value, ct);
            await _unitOfWork.CommitAsync(ct);

            return Result<Guid>.FromValue(examResult.Value.Id);
        }
    }
}
