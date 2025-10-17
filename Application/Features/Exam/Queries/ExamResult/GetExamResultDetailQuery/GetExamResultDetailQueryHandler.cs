using Application.Features.Exam.Dtos;
using AutoMapper;
using Domain.Common.Results;
using Infrastructure.Interface;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Exam.Queries.ExamResult.GetExamResultDetailQuery
{
    public class GetExamResultDetailQueryHandler : IRequestHandler<GetExamResultDetailQuery, Result<ExamResultDetailDto>>
    {
        private readonly IExamResultRepository _examResultRepository;
        private readonly IMapper _mapper;

        public GetExamResultDetailQueryHandler(
            IExamResultRepository examResultRepository,
            IMapper mapper)
        {
            _examResultRepository = examResultRepository;
            _mapper = mapper;
        }

        public async Task<Result<ExamResultDetailDto>> Handle(GetExamResultDetailQuery request, CancellationToken ct)
        {
            try
            {
                // Validate exam result ID
                if (request.ExamResultId == Guid.Empty)
                {
                    return Result<ExamResultDetailDto>.FromError(
                        Error.Validation("ExamResultId.Empty", "Exam Result ID is required."));
                }

                // Get exam result with details
                var examResult = await _examResultRepository.GetByIdWithDetailsAsync(request.ExamResultId, ct);

                if (examResult == null)
                {
                    return Result<ExamResultDetailDto>.FromError(
                        Error.NotFound("ExamResult.NotFound", "Exam result not found."));
                }

                // Map to detailed DTO
                var examResultDetailDto = _mapper.Map<ExamResultDetailDto>(examResult);

                return Result<ExamResultDetailDto>.FromValue(examResultDetailDto);
            }
            catch (Exception ex)
            {
                return Result<ExamResultDetailDto>.FromError(
                    Error.Unexpected("ExamResult.DetailQueryFailed", $"Failed to retrieve exam result details: {ex.Message}"));
            }
        }
    }
}
