using Application.Features.Exam.Dtos;
using Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Exam.Commands.Questions.UpdateQuestions
{
    public record UpdateQuestionCommand(
     Guid QuestionId,
     string Text,
     decimal Points,
     int Order
 ) : IRequest<Result<QuestionDto>>;
}
