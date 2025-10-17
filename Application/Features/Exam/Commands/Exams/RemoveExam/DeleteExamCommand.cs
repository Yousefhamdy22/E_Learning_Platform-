using Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Exam.Commands.Exams.RemoveExam
{
    public record DeleteExamCommand(Guid Id) : IRequest<Result<Success>>;
}
