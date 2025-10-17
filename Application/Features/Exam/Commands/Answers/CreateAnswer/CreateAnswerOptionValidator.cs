using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Exam.Commands.Answers.CreateAnswer
{
    public class CreateAnswerOptionValidator : AbstractValidator<CreateAnswerOptionCommand>
    {
        public CreateAnswerOptionValidator()
        {
            RuleFor(x => x.QuestionId)
                .NotEmpty().WithMessage("QuestionId is required");

            RuleFor(x => x.Text)
                .NotEmpty().WithMessage("Text cannot be empty")
                .MaximumLength(200).WithMessage("Text cannot exceed 200 characters");
        }
    }
}
