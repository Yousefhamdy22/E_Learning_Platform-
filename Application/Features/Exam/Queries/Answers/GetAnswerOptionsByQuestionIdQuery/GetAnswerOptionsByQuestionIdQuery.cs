using Application.Features.Exam.Dtos;
using Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Exam.Queries.Answers.GetAnswerOptionsByQuestionIdQuery
{
    public class GetAnswerOptionsByQuestionIdQuery : IRequest<Result<List<AnswerOptionDto>>>
    {
        public Guid QuestionId { get; set; }

        public GetAnswerOptionsByQuestionIdQuery(Guid questionId)
        {
            QuestionId = questionId;
        }
    }
}
