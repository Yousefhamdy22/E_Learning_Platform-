using Application.Features.Exam.Dtos;
using Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Exam.Commands.Exams.CreateExam
{
    public class CreateExamCommand : IRequest<Result<Guid>>
    {
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public Guid CourseId { get; set; }
        public int DurationMinutes { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<QuestionDto> Questions { get; set; } = new();
    }
}
