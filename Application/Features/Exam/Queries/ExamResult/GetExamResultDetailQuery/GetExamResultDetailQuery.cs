using Application.Features.Exam.Dtos;
using Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Exam.Queries.ExamResult.GetExamResultDetailQuery
{
    public record GetExamResultDetailQuery : IRequest<Result<ExamResultDetailDto>>
    {
        public Guid ExamResultId { get; init; }
    }
}
