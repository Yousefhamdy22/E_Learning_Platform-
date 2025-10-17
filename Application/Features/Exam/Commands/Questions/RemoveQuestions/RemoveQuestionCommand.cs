using Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Exam.Commands.Questions.RemoveQuestions
{
    public record RemoveQuestionCommand(Guid QuestionId) : IRequest<Result<Success>>;
}
