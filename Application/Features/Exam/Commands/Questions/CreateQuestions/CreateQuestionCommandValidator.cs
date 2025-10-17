using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Exam.Commands.Questions.CreateQuestions
{
    public class CreateQuestionCommandValidator : AbstractValidator<CreateQuestionCommand>
    {
        public CreateQuestionCommandValidator()
        {
            //RuleFor(x => x.ExamId)
            //    .NotEmpty().WithMessage("ExamId is required");

            RuleFor(x => x.Text)
                .NotEmpty().WithMessage("Text is required")
                .MaximumLength(500);

            RuleFor(x => x.Points)
                .GreaterThan(0).WithMessage("Points must be greater than zero");

            RuleFor(x => x.Order)
                .GreaterThanOrEqualTo(0);
        }
    }
}
