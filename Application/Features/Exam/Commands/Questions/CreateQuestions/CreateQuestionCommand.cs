using Application.Features.Exam.Dtos;
using Domain.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;


namespace Application.Features.Exam.Commands.Questions.CreateQuestions
{
    public class CreateQuestionCommand : IRequest<Result<QuestionDto>>
    {
     
        public string Text { get; set; } = string.Empty;
        public int Points { get; set; }
        public int Order { get; set; }
        public IFormFile? Image { get; set; } 
    }
}
