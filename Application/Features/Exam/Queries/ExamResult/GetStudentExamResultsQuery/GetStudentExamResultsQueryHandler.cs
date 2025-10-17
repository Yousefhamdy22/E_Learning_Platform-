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

namespace Application.Features.Exam.Queries.ExamResult.GetStudentExamResultsQuery
{
    public class GetStudentExamResultsQueryHandler : IRequestHandler<GetStudentExamResultsQuery, Result<List<ExamResultDto>>>
    {
        private readonly IExamResultRepository _examResultRepository;
        private readonly IMapper _mapper;

        public GetStudentExamResultsQueryHandler(
            IExamResultRepository examResultRepository,
            IMapper mapper)
        {
            _examResultRepository = examResultRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<ExamResultDto>>> Handle(GetStudentExamResultsQuery request, CancellationToken ct)
        {
            try
            {
                // Validate student ID
                if (request.StudentId == Guid.Empty)
                {
                    return Result<List<ExamResultDto>>.FromError(
                        Error.Validation("StudentId.Empty", "Student ID is required."));
                }

                // Get exam results for student
                var examResults = await _examResultRepository.GetByStudentIdAsync(request.StudentId, ct);

                if (!examResults.Any())
                {
                    return Result<List<ExamResultDto>>.FromValue(new List<ExamResultDto>());
                }

                // Map to DTOs
                var examResultDtos = _mapper.Map<List<ExamResultDto>>(examResults);

                return Result<List<ExamResultDto>>.FromValue(examResultDtos);
            }
            catch (Exception ex)
            {
                return Result<List<ExamResultDto>>.FromError(
                    Error.Unexpected("ExamResult.QueryFailed", $"Failed to retrieve exam results: {ex.Message}"));
            }
        }
    }
}
