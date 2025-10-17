using Domain.Common.Results;
using MediatR;
using System;


namespace Application.Features.Exam.Commands.Answers.CreateAnswer
{
    public sealed record CreateAnswerOptionCommand(
      Guid QuestionId,
      string Text,
      bool IsCorrect
  ) : IRequest<Result<Guid>>;
}
