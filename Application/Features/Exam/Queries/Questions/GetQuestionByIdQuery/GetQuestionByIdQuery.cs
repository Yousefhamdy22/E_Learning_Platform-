using Application.Features.Exam.Dtos;
using Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Exam.Queries.Questions.GetQuestionByIdQuery
{
    public record GetQuestionByIdQuery(Guid QuestionId)
     : IRequest<Result<QuestionDto>>
    {
        public Guid QuestionId { get; set; } = QuestionId;
    }
}
