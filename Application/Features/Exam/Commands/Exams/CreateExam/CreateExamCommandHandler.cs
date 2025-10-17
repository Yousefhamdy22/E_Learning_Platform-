using Application.Features.Exam.Dtos;
using AutoMapper;
using Domain.Common.Interface;
using Domain.Common.Results;
using Domain.Entities.Exams;
using Infrastructure.Interface;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Exam.Commands.Exams.CreateExam
{
    public class CreateExamCommandHandler : IRequestHandler<CreateExamCommand, Result<Guid>>
    {
        private readonly IExamRepository _examRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly HybridCache _cache;

        public CreateExamCommandHandler(
            IExamRepository examRepository,
            IQuestionRepository questionRepository,
            IUnitOfWork unitOfWork,
            HybridCache cache)
        {
            _examRepository = examRepository;
            _questionRepository = questionRepository;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<Result<Guid>> Handle(CreateExamCommand request, CancellationToken ct)
        {
            // 1. Create Exam
            var examResult = Domain.Entities.Exams.Exam.Create(
                request.Title,
                request.Description,
                request.CourseId,
                request.DurationMinutes,
                request.StartDate,
                request.EndDate);

            if (examResult.IsError)
                return Result<Guid>.FromError(examResult.TopError);

            var exam = examResult.Value;

            // 2. Process Questions
            var examQuestions = new List<ExamQuestions>();
            int order = 1;

          
            foreach (var qRequest in request.Questions)
            {
                Result<Question> questionResult;
                Question question;

                
                var existingQuestion = await _questionRepository.GetByIdAsync(qRequest.Id, ct);

                if (existingQuestion != null)
                {
                    
                    question = existingQuestion;
                }
                else
                {
                   
                    questionResult = Question.Create(qRequest.Text, qRequest.Point);

                    if (questionResult.IsError)
                        return Result<Guid>.FromError(questionResult.TopError);

                    question = questionResult.Value;

                  
                    foreach (var ans in qRequest.Answers)
                    {
                        question.AddAnswerOption(ans.Text, ans.IsCorrect);
                    }

                  
                    var validationResult = question.ValidateMultipleChoice();
                    if (validationResult.IsError)
                        return Result<Guid>.FromError(validationResult.TopError);

                    
                    await _questionRepository.AddAsync(question, ct);
                }

                var examQuestion = ExamQuestions.Create(exam.Id, question.Id, order++);
                examQuestions.Add(examQuestion);

             
                exam.AddExamQuestion(examQuestion);
            }

            await _examRepository.AddAsync(exam, ct);
            await _unitOfWork.CommitAsync(ct);

            await _cache.RemoveAsync($"exam:list:{request.CourseId}", ct);

            return Result<Guid>.FromValue(exam.Id);
           
        }
    }
}













