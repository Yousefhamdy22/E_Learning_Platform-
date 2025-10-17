using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Exam.Commands.Exams.CreateExam
{
    public class CreateExamCommandValidator : AbstractValidator<CreateExamCommand>
    {
        public CreateExamCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Exam title is required")
                .MaximumLength(200);

            RuleFor(x => x.CourseId)
                .NotEmpty();

            RuleFor(x => x.DurationMinutes)
                .GreaterThan(0);

            RuleFor(x => x.StartDate)
                .LessThan(x => x.EndDate);

            RuleForEach(x => x.Questions).ChildRules(q =>
            {
                q.RuleFor(x => x.Text).NotEmpty();
                q.RuleFor(x => x.Point).GreaterThan(0);

                q.RuleForEach(x => x.Answers).ChildRules(a =>
                {
                    a.RuleFor(x => x.Text).NotEmpty();
                });
            });
        }
    }
}
