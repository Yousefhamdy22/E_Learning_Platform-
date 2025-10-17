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

namespace Application.Features.Courses.Commands.RemoveCourse
{
    public class DeleteCourseCommandHandler : IRequestHandler<DeleteCourseCommand, Result<Success>>
    {
        private readonly ICourseRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly HybridCache _cache;

        public DeleteCourseCommandHandler(ICourseRepository repository, IUnitOfWork unitOfWork, HybridCache cache)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<Result<Success>> Handle(DeleteCourseCommand request, CancellationToken ct)
        {
            var course = await _repository.GetByIdAsync(request.Id, ct);
            if (course == null) return Result<Success>.FromError(Error.NotFound("Course not found"));

            var result = course.Delete();
            if (result.IsError) return result;

            await _repository.DeleteAsync(course , ct);
            await _unitOfWork.CommitAsync(ct);
            await _cache.RemoveAsync($"Course_{course.Id}");

            return Result<Success>.FromValue(new Success());
        }
    }
}
