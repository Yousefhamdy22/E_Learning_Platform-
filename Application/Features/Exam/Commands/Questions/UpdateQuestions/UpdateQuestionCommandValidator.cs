using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Exam.Commands.Questions.UpdateQuestions
{
    public class UpdateQuestionCommandValidator : AbstractValidator<UpdateQuestionCommand>
    {
        public UpdateQuestionCommandValidator()
        {
            RuleFor(x => x.QuestionId).NotEmpty();
            RuleFor(x => x.Text)
                .NotEmpty().MaximumLength(500);
            RuleFor(x => x.Points)
                .GreaterThan(0);
            RuleFor(x => x.Order)
                .GreaterThanOrEqualTo(0);
        }
    }
}
