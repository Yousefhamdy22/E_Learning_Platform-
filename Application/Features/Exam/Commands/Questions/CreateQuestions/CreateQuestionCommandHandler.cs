using Application.Features.Exam.Dtos;
using AutoMapper;
using Domain.Common.Interface;
using Domain.Common.Results;
using Domain.Entities.Exams;
using Infrastructure.Helper.FileStorage;
using Infrastructure.Helper.FileStorage.Validations;
using Infrastructure.Interface;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Exam.Commands.Questions.CreateQuestions
{
    public class CreateQuestionCommandHandler
    : IRequestHandler<CreateQuestionCommand, Result<QuestionDto>>
    {

        #region DI 
        

        private readonly IExamRepository _examRepository;
        private readonly ILogger<CreateQuestionCommandHandler> _logger;
        private readonly IFileService _fileService ;
        private readonly FileValidation _fileValidation;
        private readonly IQuestionRepository _questionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HybridCache _cache;

        #endregion

        #region Ctors 
        
        public CreateQuestionCommandHandler(
            IExamRepository examRepository,
            IFileService fileService,
            FileValidation fileValidation,
            IQuestionRepository questionRepository,
            ILogger<CreateQuestionCommandHandler> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            HybridCache cache)
        {
            _examRepository = examRepository;
            _questionRepository = questionRepository;
            _fileService = fileService;
            _fileValidation = fileValidation;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _cache = cache;
        }
        #endregion

        #region Handling 
     
        public async Task<Result<QuestionDto>> Handle(CreateQuestionCommand request, CancellationToken ct)
        {
            try
            {
                // 1. Check if Exam exists
                //var exam = await _examRepository.GetByIdAsync(request.ExamId, ct);
                //if (exam == null)
                //    return Result<QuestionDto>.FromError(
                //        Error.NotFound("Exam.NotFound", "Exam does not exist."));

                string? imageUrl = null;

                // 2. Upload image if provided
                if (request.Image != null && request.Image.Length > 0)
                {
                    var uploadResult =await _fileValidation.UploadQuestionImageAsync(request.Image, ct);
                    if (uploadResult.IsSuccess)
                    {
                        imageUrl = uploadResult.Value;
                        _logger.LogInformation("Image uploaded for question: {ImageUrl}", imageUrl);
                    }
                    else
                    {
                        _logger.LogWarning("Failed to upload question image: {Error}", uploadResult.Errors);
                       
                    }
                }

                // 3. Create Question (Domain Factory)
                var result = Question.Create( request.Text, request.Points);
                if (result.IsError)
                    return Result<QuestionDto>.FromError(Error.Failure("Create Question Domain"));

                var question = result.Value;

                // 4. Set image URL if uploaded
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    question.UpdateImage(imageUrl);
                }

                // 5. Persist Question
                await _questionRepository.AddAsync(question, ct);
                await _unitOfWork.CommitAsync(ct);

                // 6. Map to DTO
                var dto = _mapper.Map<QuestionDto>(question);

                // 7. Update Cache (Hybrid Caching)
                await _cache.SetAsync($"Exam_{request.Text}_Question_{question.Id}", dto);

                _logger.LogInformation("Question created with ID: {QuestionId}", question.Id);
                return Result<QuestionDto>.FromValue(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating question for exam {ExamId}", request.Text);
                return Result<QuestionDto>.FromError(Error.Failure("Question.CreationFailed", ex.Message));
            }
        }
        #endregion
    }
}
