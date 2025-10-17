using Application.Features.Exam.Dtos;
using AutoMapper;
using Domain.Common.Interface;
using Domain.Common.Results;
using Infrastructure.Interface;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Exam.Commands.Exams.SubmitExam
{
    public class SubmitExamCommandHandler : IRequestHandler<SubmitExamCommand, Result<ExamResultDto>>
    {
        private readonly IExamResultRepository _examResultRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SubmitExamCommandHandler(
            IExamResultRepository examResultRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _examResultRepository = examResultRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<ExamResultDto>> Handle(SubmitExamCommand request, CancellationToken ct)
        {
            var examResult = await _examResultRepository.GetByIdWithDetailsAsync(request.ExamResultId, ct);
            if (examResult == null)
                return Result<ExamResultDto>.
                    FromError(Error.NotFound("ExamResult.NotFound", "Exam result not found."));

            // Finalize exam submission
            var submitResult = examResult.SubmitExam();
            if (submitResult.IsError)
                return Result<ExamResultDto>.FromError(submitResult.TopError);

            // Update final statistics
            examResult.UpdateStatistics();

            await _unitOfWork.CommitAsync(ct);

            var dto = _mapper.Map<ExamResultDto>(examResult);
            return Result<ExamResultDto>.FromValue(dto);
        }
    }
}
