using Application.Features.Exam.Dtos;
using Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Exam.Commands.Exams.SubmitExam
{
    public class SubmitExamCommand : IRequest<Result<ExamResultDto>>
    {
        public Guid ExamResultId { get; set; }
    }
}
