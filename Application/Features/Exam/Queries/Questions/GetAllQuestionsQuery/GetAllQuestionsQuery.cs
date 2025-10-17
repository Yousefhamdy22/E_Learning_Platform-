using Application.Features.Exam.Dtos;
using Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Exam.Queries.Questions.GetAllQuestionsQuery
{
    public record GetAllQuestionsQuery : IRequest<Result<List<QuestionDto>>>;
}
