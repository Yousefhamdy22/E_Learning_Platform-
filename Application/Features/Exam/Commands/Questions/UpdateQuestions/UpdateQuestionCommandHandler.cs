using Application.Features.Exam.Dtos;
using AutoMapper;
using Domain.Common.Interface;
using Domain.Common.Results;
using Infrastructure.Interface;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Exam.Commands.Questions.UpdateQuestions
{
    public class UpdateQuestionCommandHandler
     : IRequestHandler<UpdateQuestionCommand, Result<QuestionDto>>
    {
        private readonly IQuestionRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HybridCache _cache;

        public UpdateQuestionCommandHandler(
            IQuestionRepository repository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            HybridCache cache)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<Result<QuestionDto>> Handle(UpdateQuestionCommand request, CancellationToken ct)
        {
           
            var question = await _repository.GetByIdAsync(request.QuestionId, ct);
            if (question == null)
                return Result<QuestionDto>.FromError(Error.NotFound("Question.NotFound", "Question not found"));

            
            question.SetText(request.Text);
            question.SetPoints(request.Points);
            //question.Order = request.Order; 

          
            await _repository.UpdateAsync(question , ct);
            await _unitOfWork.CommitAsync(ct);

           
            var dto = _mapper.Map<QuestionDto>(question);

           
            //await _cache.SetAsync($"Exam_{question.ExamId}_Question_{question.Id}", dto);

            return Result<QuestionDto>.FromValue(dto);
        }
    }
}
