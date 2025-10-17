using Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Exam.Commands.Exams.StartExam
{
    public class StartExamCommand : IRequest<Result<Guid>>
    {
        public Guid StudentId { get; set; }
        public Guid ExamId { get; set; }
    }
   
}
