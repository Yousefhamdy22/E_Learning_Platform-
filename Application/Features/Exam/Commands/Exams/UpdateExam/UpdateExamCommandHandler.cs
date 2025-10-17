using Application.Features.Exam.Dtos;
using AutoMapper;
using Domain.Common.Interface;
using Domain.Common.Results;
using Infrastructure.Interface;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Exam.Commands.Exams.UpdateExam
{
    public class UpdateExamCommandHandler : IRequestHandler<UpdateExamCommand, Result<ExamDto>>
    {
        private readonly IExamRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HybridCache _cache;

        public UpdateExamCommandHandler(IExamRepository repository, IUnitOfWork unitOfWork,
            IMapper mapper, HybridCache cache)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<Result<ExamDto>> Handle(UpdateExamCommand request, CancellationToken ct)
        {
            var exam = await _repository.GetByIdAsync(request.Id, ct);
            if (exam == null) return Result<ExamDto>.FromError(Error.NotFound("Exam not found"));

            var result = exam.Update(request.Title, request.Description,
                request.DurationMinutes, request.StartDate, request.EndDate);

            if (result.IsError) return Result<ExamDto>.FromError(Error.Failure("Error With Update Exam "));

            await _unitOfWork.CommitAsync(ct);

            var dto = _mapper.Map<ExamDto>(exam);
            await _cache.SetAsync($"Exam_{exam.Id}", dto);

            return Result<ExamDto>.FromValue(dto);
        }
    }
}
