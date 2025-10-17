using Application.Features.Exam.Dtos;
using Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Exam.Commands.Exams.UpdateExam
{
    public record UpdateExamCommand(
     Guid Id,
     string Title,
     string? Description,
     int DurationMinutes,
     DateTimeOffset StartDate,
     DateTimeOffset EndDate
 ) : IRequest<Result<ExamDto>>;
}
