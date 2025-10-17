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

namespace Application.Features.Instructors.Command.DeleteInstructor
{
    public class DeleteInstructorCommandHandler : IRequestHandler<DeleteInstructorCommand, Result<bool>>
    {
        private readonly IInstructorRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly HybridCache _cache;

        public DeleteInstructorCommandHandler(IInstructorRepository repository,
                                              IUnitOfWork unitOfWork,
                                              HybridCache cache)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<Result<bool>> Handle(DeleteInstructorCommand request, CancellationToken ct)
        {
            var instructor = await _repository.GetByIdAsync(request.Id, ct);
            if (instructor == null)
                return Result<bool>.FromError(Error.NotFound("Instructor not found"));

            await _repository.DeleteAsync(instructor , ct);
            await _unitOfWork.CommitAsync(ct);

         
           await _cache.RemoveAsync($"Instructor_{request.Id}");

            return Result<bool>.FromValue(true);
        }
    }

}
